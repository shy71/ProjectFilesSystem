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
        List<string> disks = new List<string>();
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                var wr = new ItemPanel();
                wr.DoubleClick += OpenDisk;
                adr.SetText("C:\\");
                myList.Items.Add(wr);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        static public List<string> GetDisksNames(string path = "../Debug/")
        {
            return Directory.GetFiles(path).Where(x => x.Substring(x.Length - 4) == ".dsk").Select(x => x.Substring(x.IndexOf('.') + 9)).Select(y => y.Substring(0, y.IndexOf(".dsk"))).ToList();

        }
        private void OpenDisk(object sender, EventArgs e)
        {
            try
            {
                OpenDisk((sender as Button).Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void OpenDisk(string name)
        {
            try
            {
                myList.Items.Clear();
                var wr = new ItemPanel(name);
                wr.DoubleClick += OpenFile;
                myList.Items.Add(wr);
                adr.SetText(adr.GetText() + name + "\\");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void OpenFile(object sender, EventArgs e)
        {
            try
            {
                //myList.Items.Clear();
                adr.SetText(adr.GetText() + (sender as Button).Name + "\\");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreteDskMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((ItemPanel)myList.Items.GetItemAt(0)).RootLevel())
                {
                    new NewDisk().ShowDialog();
                    (myList.Items.GetItemAt(0) as ItemPanel).Refresh();
                }
                else
                    MessageBox.Show("You cant cretae a new disk from inside a file");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void CreteNewFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (myList.Items.GetItemAt(0) as ItemPanel).CreateFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void SelectedItemProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = ((ItemPanel)myList.Items.GetItemAt(0)).GetFocused();
                if (((ItemPanel)myList.Items.GetItemAt(0)).RootLevel())
                {
                    Disk d = new FMS_adapter.Disk();
                    d.MountDisk(name);
                    MessageBox.Show("Disk name: " + d.GetName()
                                  + "\nDisk owner: " + d.GetOwner()
                                  + "\nCreation date: " + d.GetCreationDate()
                                  + "\n" + App.NumByteToString(d.HowMuchEmpty() * 1020) + " free of " + App.NumByteToString(1024 * 1600), "Properties", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else //file type
                {
                    if (UserName.GetText() == null)
                    {
                        MessageBox.Show("You need to enter your username!");
                        return;
                    }
                    FCB fcb = ((ItemPanel)myList.Items.GetItemAt(0)).GetDisk().OpenFile(name, UserName.GetText(), "I");
                    DirEntry entry = fcb.GetDirEntry();
                    MessageBox.Show("\nFile name: " + entry.Filename
                                            + "\nFile owner: " + entry.FileOwner
                                            + "\nFile Size: " + entry.FileSize * 1020
                                            + "\nCreation date: " + entry.CrDate
                                            + "\nFile key type: " + ((entry.KeyType == "I") ? "integer" : "string"), "Properties", MessageBoxButton.OK, MessageBoxImage.Information);
                    fcb.CloseFile();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = ((ItemPanel)myList.Items.GetItemAt(0)).GetFocused();
                if (((ItemPanel)myList.Items.GetItemAt(0)).RootLevel())
                {
                    Disk d = new FMS_adapter.Disk();
                    File.Delete(name + ".dsk");
                    ((ItemPanel)myList.Items.GetItemAt(0)).Refresh();

                }
                else //file type
                {
                    if (UserName.GetText() == null)
                    {
                        MessageBox.Show("You need to enter your username!");
                        return;
                    }
                    ((ItemPanel)myList.Items.GetItemAt(0)).GetDisk().DelFile(name, UserName.GetText());
                    ((ItemPanel)myList.Items.GetItemAt(0)).Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void OpenClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((ItemPanel)myList.Items.GetItemAt(0)).RootLevel())
                    OpenDisk(((ItemPanel)myList.Items.GetItemAt(0)).GetFocused());
                else
                    ((ItemPanel)myList.Items.GetItemAt(0)).OpenFile(((ItemPanel)myList.Items.GetItemAt(0)).GetFocused());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UserName_Changed(object sender, EventValue e)
        {
            try
            {
                ((ItemPanel)myList.Items.GetItemAt(0)).Username = (sender as TextControl).GetText();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
