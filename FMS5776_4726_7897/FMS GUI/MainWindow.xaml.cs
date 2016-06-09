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
using System.IO;
using FMS_adapter;

namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> disks=new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            var wr = new ItemPanel();
            wr.OpenDiskEvent += OpenDisk;
            adr.SetText("C:/");
            myList.Items.Add(new ItemPanel());
            
        }
        private void OpenDisk(object sender, EventArgs e)
        {
            //opening disk
        }

        private void CreteDskMenu_Click(object sender, RoutedEventArgs e)
        {
            new NewDisk().ShowDialog();
            
        }

        private void SelectedItemProperties_Click(object sender, RoutedEventArgs e)
        {
            string name = ((ItemPanel)myList.Items.GetItemAt(0)).GetFocused();
            //finish
        }

        private void SelectedItemClose_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
