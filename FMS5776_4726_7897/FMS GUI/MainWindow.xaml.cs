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
            wr.DoubleClick += OpenDisk;
            adr.SetText("C:\\");
            myList.Items.Add(wr);
            
        }
        static public List<string> GetDisksNames(string path = "../Debug/")
        {
            return Directory.GetFiles(path).Where(x => x.Substring(x.Length - 4) == ".dsk").Select(x => x.Substring(x.IndexOf('.') + 9)).Select(y => y.Substring(0, y.IndexOf(".dsk"))).ToList();
        }
        /*static public List<string> GetDiskFileNames(Disk d)
        {
            uint address = d.GetVolumeHeader().AddrRootDir;


        }*/
        private void OpenDisk(object sender, EventArgs e)
        {
            myList.Items.Clear();
            var wr = new ItemPanel((sender as Button).Name);
            wr.DoubleClick += OpenFile;
            myList.Items.Add(wr);
            adr.SetText(adr.GetText()+ (sender as Button).Name+"\\");
        }
        private void OpenFile(object sender, EventArgs e)
        {
            myList.Items.Clear();
            //inside file
            adr.SetText(adr.GetText() + (sender as Button).Name + "\\");
        }
        private void CreteDskMenu_Click(object sender, RoutedEventArgs e)
        {
            new NewDisk().ShowDialog();
            (myList.Items.GetItemAt(0) as ItemPanel).Refresh();
            
        }
        private void CreteNewFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (myList.Items.GetItemAt(0) as ItemPanel).OpenFile();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        
        private void SelectedItemProperties_Click(object sender, RoutedEventArgs e)
        {
            string name = ((ItemPanel)myList.Items.GetItemAt(0)).GetFocused();
            if(GetDisksNames().Find(x => x == name) != null)
            {
                Disk d = new FMS_adapter.Disk();
                d.MountDisk(name);
                MessageBox.Show("Disk name: " + d.GetName()
                              + "\nDisk owner: " + d.GetOwner()
                              + "\nCreation date: " + d.GetCreationDate()
                              + "\n" + App.NumByteToString(d.HowMuchEmpty()*1020) + " free of "+App.NumByteToString(1024*1600),"Properties",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            //finish
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (adr.GetText().Count(x => x == '\\') == 1)
                MessageBox.Show("You are already in the root level");
            else
            {
                myList.Items.Clear();
                var wr = new ItemPanel();
                wr.DoubleClick += OpenDisk;
                adr.SetText("C:\\");
                myList.Items.Add(wr);
            }

        }
    }
}
