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
using FMS_adapter;

namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for Create_Record.xaml
    /// </summary>
    public partial class Create_Record : Window
    {
        StringBuilder record;
        FieldItem field;
        DirEntry dirEntry;
        public Create_Record()
        {
            InitializeComponent();
        }
        public Create_Record(ref StringBuilder s,DirEntry dirEntry)
        {
            this.dirEntry = dirEntry;
            InitializeComponent();
            record = s;
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
            bool foundKey = false,goodKey=true,goodlength=true;
            foreach(FieldItem f in Fields.Children)
            {
                if (f.Name == (string)KeyField.SelectedItem)
                {
                    string s = f.Name + "," + f.Content + "." + record.ToString();
                    record.Clear();
                    record.Append(s);
                    foundKey = true;
                    if (dirEntry.KeyType == "I")//integer type
                    {
                        int keyNum;
                        bool isInteger = int.TryParse(f.Content, out keyNum);
                        if (!isInteger)
                        {
                            goodKey = false;
                            MessageBox.Show("That is an invalid key, since it must be of integer type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    if(f.Content.Length != dirEntry.KeySize)
                    {
                        goodlength = false;
                        MessageBox.Show("That is an invalid key, since it must be The correct length.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    //Maybe add relevence to the offset. for now we always just put the key field in the begining, therefore we want the offset to always be 0. so we'll just have to show the teacher it that way
                }
                else
                    record.Append(f.Name + "," + f.Content + ".");
            }
            if(!foundKey || !goodKey || !goodlength)
            {
                MessageBox.Show("You haven't chosen a  key or it is an invalid key...\n", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                record.Clear();
            }
            else if (record.Length > dirEntry.MaxRecSize)
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
