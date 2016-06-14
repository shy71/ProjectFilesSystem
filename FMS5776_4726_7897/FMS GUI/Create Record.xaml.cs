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

        private void AddField_Click(object sender, RoutedEventArgs e)
        {
            FieldItem field = new FieldItem();
            Fields.Children.Add(field);
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            foreach(FieldItem f in Fields.Children)
            {
                record.Append(f.Name + "," + f.Content + ".");
            }
            if (record.Length > MaxSize)
            {
                MessageBox.Show("You exceeded the maximum record length...\nTry taking off a few fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EraseField_Click(object sender, RoutedEventArgs e)
        {
            //finish
        }
    }
}
