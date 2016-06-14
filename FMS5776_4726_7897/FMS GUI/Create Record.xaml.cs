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
    /// Interaction logic for Create_Record.xaml
    /// </summary>
    public partial class Create_Record : Window
    {
        StringBuilder record;
        FieldItem field;
        int MaxSize;
        public Create_Record()
        {
            InitializeComponent();
        }
        public Create_Record(ref StringBuilder s,int maxsize)
        {
            InitializeComponent();
            record = s;
            MaxSize = maxsize;
        }
        public void fillUpKeyFieldCombobox()
        {
            KeyField.Items.Clear();
            foreach(FieldItem f in Fields.Children)
                if(f.Name != "")
                    KeyField.Items.Add(f.Name);
        }
        private void AddField_Click(object sender, RoutedEventArgs e)
        {
            field = new FieldItem();
            Fields.Children.Add(field);
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            bool foundKey = false;
            foreach(FieldItem f in Fields.Children)
            {
                if (f.Name == KeyField.SelectedItem)
                {
                    string s = f.Name + "," + f.Content + "." + record.ToString();
                    record.Clear();
                    record.Append(s);
                    foundKey = true;
                }
                else
                    record.Append(f.Name + "," + f.Content + ".");
            }
            if(!foundKey)
            {
                MessageBox.Show("You haven't chosen a key...\n", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                record.Clear();
            }
            else if (record.Length > MaxSize)
            {
                MessageBox.Show("You exceeded the maximum record length...\nTry taking off a few fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                record.Clear();
            }
            else
                this.Close();
        }

        private void EraseField_Click(object sender, RoutedEventArgs e)
        {
            Fields.Children.Remove(field);
            if (Fields.Children.Count > 0)
                field = (FieldItem)Fields.Children[Fields.Children.Count - 1];
            fillUpKeyFieldCombobox();
        }

        private void KeyField_DropDownOpened(object sender, EventArgs e)
        {
            fillUpKeyFieldCombobox();
        }
    }
}
