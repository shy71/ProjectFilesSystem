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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for FieldItem.xaml
    /// </summary>
    public partial class FieldItem : UserControl
    {
        public string Name, Content;
        public FieldItem()
        {
            InitializeComponent();
            Name = Content = "";
        }

        private void name_LostFocus(object sender, RoutedEventArgs e)
        {
            Name = ((TextBox)sender).Text.ToString();
        }

        private void content_LostFocus(object sender, RoutedEventArgs e)
        {
            Content = ((TextBox)sender).Text.ToString();
        }
    }
}
