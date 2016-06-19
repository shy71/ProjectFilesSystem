using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for Opening_Record.xaml
    /// </summary>
    public partial class Opening_Record : Window
    {
        List<string> fields;
        int maxSize;
        StringBuilder record;
        string editField;
        bool isAdding;
        public Opening_Record()
        {
            InitializeComponent();
        }
        public Opening_Record(ref StringBuilder curRecord, int maxSize)
        {
            InitializeComponent();
            this.maxSize = maxSize;
            record = curRecord;
            editField = "";
            isAdding = false;
            fields = new List<string>(record.ToString().Split('.'));
            fields.RemoveAll(x => x == "");
            Key.FieldName.Text = (fields[0].Split(','))[0];
            Key.FieldContent.Text = (fields[0].Split(','))[1];
            fields.Remove(fields[0]);
            Refresh();
        }
        public Opening_Record(ref StringBuilder curRecord, int maxSize,bool ReadOnly):this(ref curRecord,maxSize)
        {
            DelField.Visibility = Visibility.Collapsed;
            EditField.Visibility = Visibility.Collapsed;
        }
        private void DelField_Click(object sender, RoutedEventArgs e)
        {
            if (Fields.SelectedItem == null)
            {
                MessageBox.Show("You haven't seleted anything to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            UneditableField field = (UneditableField)Fields.SelectedItem;
            string f = field.FieldName.Text + "," + field.FieldContent.Text;
            fields.Remove(f);
            Refresh();
        }
        private void EditField_Click(object sender, RoutedEventArgs e)
        {
            if (Fields.SelectedItem == null)
            {
                MessageBox.Show("You haven't seleted anything to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string name = ((UneditableField)Fields.SelectedItem).FieldName.Text;
            string content = ((UneditableField)Fields.SelectedItem).FieldContent.Text;
            int index = Fields.SelectedIndex;
            editField = name + "," + content;
            Fields.Items.RemoveAt(Fields.SelectedIndex);
            FieldItem fItem = new FieldItem();
            fItem.Name = name;
            fItem.name.SetText(name);
            fItem.Content = content;
            fItem.content.SetText(content);
            Fields.Items.Insert(index, fItem);
        }
        private void Refresh()
        {
            Fields.Items.Clear();
            foreach (string field in fields)
            {
                UneditableField f = new UneditableField();
                f.FieldName.Text = field.Split(',')[0];
                f.FieldContent.Text = field.Split(',')[1];
                Fields.Items.Add(f);
            }
        }
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Would you like to save the changes you made? (if you made any)", "Save Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (res == MessageBoxResult.Cancel)
            {
                return;
            }
            else if (res == MessageBoxResult.No)
            {
                this.Close();
            }
            else
            {
                Fields_Selected(this, e);
                record.Clear();
                record.Append(Key.FieldName.Text + "," + Key.FieldContent.Text + ".");
                foreach (string field in fields)
                    record.Append(field.Split(',')[0] + "," + field.Split(',')[1] + ".");
                if (record.Length > maxSize)
                {
                    MessageBox.Show("Your record is too big.\nRecord maximum size exceeded.", "Record maximum size exceeded", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                this.Close();
            }
        }

        private void Fields_Selected(object sender, RoutedEventArgs e)
        {
            foreach (UserControl field in Fields.Items)
            {
                if (field.GetType() == typeof(FieldItem))
                {
                    int index = Fields.Items.IndexOf(field);
                    string name = ((FieldItem)field).name.textBox.Text, content = ((FieldItem)field).content.textBox.Text;
                    Fields.Items.RemoveAt(index);
                    if (isAdding)
                    {
                        fields.Add(name + "," + content);
                        isAdding = false;
                    }
                    else
                        fields[fields.FindIndex((x) => x == editField)] = name + "," + content;
                    UneditableField f = new UneditableField();
                    f.FieldName.Text = name;
                    f.FieldContent.Text = content;
                    Fields.Items.Add(f);
                    return;
                }
            }
        }

        private void AddField_Click(object sender, RoutedEventArgs e)
        {

            Fields_Selected(this,e);
            isAdding = true;
            FieldItem fItem = new FieldItem();
            Fields.Items.Add(fItem);
        }
    }
}
