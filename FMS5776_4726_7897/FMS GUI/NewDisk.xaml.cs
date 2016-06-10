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
    /// Interaction logic for NewDisk.xaml
    /// </summary>
    public partial class NewDisk : Window
    {
        public NewDisk() 
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FMS_adapter.Disk d = new FMS_adapter.Disk();
                d.Createdisk(name.GetText(), owner.GetText());
            }
            catch(Exception s)
            {
                MessageBox.Show(s.Message, "Error: Create Disk", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
        }
    }
}
