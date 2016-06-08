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
using FMS_adapter;

namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> disks;
        public MainWindow()
        {
            InitializeComponent();
            diskBox.ItemsSource = disks;
        }

        private void diskBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Disk d = new Disk();
                d.MountDisk(diskBox.SelectedItem.ToString());
                new ShowDisk(d).Show();
            }
            catch
            {
                MessageBox.Show("Error");
            }

        }

    }
}
