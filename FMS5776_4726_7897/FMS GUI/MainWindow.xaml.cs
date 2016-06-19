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
        static public List<string> GetDisksNames(string parentSub ="" )
        {
            if(parentSub=="")
            return Directory.GetFiles("../Debug/").Where(x => x.Substring(x.Length - 4) == ".dsk").Select(x => x.Substring(x.IndexOf('.') + 9)).Select(y => y.Substring(0, y.IndexOf(".dsk"))).ToList();
            else
                return Directory.GetFiles("../Debug/").Select(x => x.Substring(x.IndexOf('.') + 9)).Where(x => x.Contains(".dsk") &&x.Length>parentSub.Length&& x.Substring(x.Length-parentSub.Length) == parentSub).Select(y => y.Substring(0, y.Length-parentSub.Length-1)).Where(x => x.Substring(x.Length - 4) == ".dsk").Select(y => y.Substring(0, y.IndexOf(".dsk"))).ToList();
        }
        static public List<string> GetFolderNames(string parentSub = "")
        {
            if (parentSub == "")
                return Directory.GetFiles("../Debug/").Where(x => x.Substring(x.Length - 4) != ".dsk" && x.Contains(".dsk")).Select(y => y.Substring(y.LastIndexOf(".") + 1)).Distinct().ToList();
            else
                return Directory.GetFiles("../Debug/").Where(x => x.Contains(".dsk") &&x.Length>parentSub.Length&& x.Substring(x.Length - parentSub.Length) == parentSub).Select(y => y.Substring(0, y.Length - parentSub.Length-1)).Where(x => x.Substring(x.Length - 4) != ".dsk").Select(y => y.Substring(y.LastIndexOf(".") + 1)).Distinct().ToList();
        }
        private void OpenDisk(object sender, EventArgs e)
        {
            try
            {
                if ((sender as Button).ToolTip == "Disk")
                    OpenDisk((sender as Button).Name);
                else
                    OpenFolder((sender as Button).Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void OpenFolder(string name)
        {
            try
            {
                string str;
                if((myList.Items.GetItemAt(0) as ItemPanel).Parent=="")
                    str="";
                else
                    str="."+(myList.Items.GetItemAt(0) as ItemPanel).Parent;
                var wr = new ItemPanel(name+str,true);
                (myList.Items.GetItemAt(0) as ItemPanel).Clear();
                myList.Items.Clear();
                wr.DoubleClick += OpenDisk;
                myList.Items.Add(wr);
                adr.SetText(adr.GetText() + name + "\\");
                UserName_Changed(UserName, null);
                FormatBtn.Visibility = Visibility.Hidden;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void OpenDisk(string name)
        {
            try
            {
                var wr = new ItemPanel(name,(myList.Items.GetItemAt(0) as ItemPanel).Parent);
                (myList.Items.GetItemAt(0) as ItemPanel).Clear();
                myList.Items.Clear();              
                wr.DoubleClick += OpenFile;
                myList.Items.Add(wr);
                adr.SetText(adr.GetText() + name + "\\");
                UserName_Changed(UserName, null);
                FormatBtn.Visibility = Visibility.Visible;
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
                FormatBtn.Visibility = Visibility.Hidden;
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
                if (((ItemPanel)myList.Items.GetItemAt(0)).InFolder())
                {
                    new NewDisk(((ItemPanel)myList.Items.GetItemAt(0)).Parent).ShowDialog();
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
                if ((myList.Items.GetItemAt(0) as ItemPanel).FolderFocused())
                {
                    string path;
                    if((myList.Items.GetItemAt(0) as ItemPanel).Parent=="")
                        path=name;
                    else
                        path=name+ "."+(myList.Items.GetItemAt(0) as ItemPanel).Parent;
                    string disks = string.Join(" , ", GetDisksNames(path));
                    string folders = string.Join(" , ", GetFolderNames(path));
                      MessageBox.Show("Folder Name: "+name
                          + "\nNumber Of Files: " + GetDisksNames(path).Count()
                          + "\nFiles: "+ disks
                          + "\nNumber Of Subfolders: " + GetFolderNames(path).Count()
                          + "\nSubfolders: " + folders ,"Prperties",MessageBoxButton.OK,MessageBoxImage.Information);
                }
                else  if (((ItemPanel)myList.Items.GetItemAt(0)).InFolder())
                {
                    Disk d = new FMS_adapter.Disk();
                    if((myList.Items.GetItemAt(0) as ItemPanel).Parent!="")
                        d.SetEnd(".dsk."+(myList.Items.GetItemAt(0) as ItemPanel).Parent);
                    d.MountDisk(name);
                    MessageBox.Show("Disk name: " + d.GetName()
                                  + "\nDisk owner: " + d.GetOwner()
                                  + "\nCreation date: " + d.GetCreationDate()
                                  +((d.GetFormatDate()==null)?"":"\nFormat Date: " +d.GetFormatDate())
                                  + "\n" + App.NumByteToString(d.HowMuchEmpty() * 1020) + " free of " + App.NumByteToString(1024 * 1600), "Properties", MessageBoxButton.OK, MessageBoxImage.Information);
                d.UnmountDisk();
                }
                else //file type
                {

                    FCB fcb = ((ItemPanel)myList.Items.GetItemAt(0)).GetDisk().OpenFile(name,"System" , "I");
                    DirEntry entry = fcb.GetDirEntry();
                    MessageBox.Show("\nFile name: " + entry.Filename
                                            + "\nFile owner: " + entry.FileOwner
                                            + "\nFile Size: " + App.NumByteToString(entry.FileSize * 1020)
                                            + "\nCreation date: " + entry.CrDate
                                            +"\nRecord Size: " + entry.MaxRecSize
                                            + "\nFile key type: " + ((entry.KeyType == "I") ? "integer" : "string")
                                            + "\n" + App.NumByteToString(1020 * entry.FileSize - entry.EofRecNum * entry.MaxRecSize) + " free of " + App.NumByteToString(1020 * entry.FileSize), "Properties", MessageBoxButton.OK, MessageBoxImage.Information);
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
                else if ((myList.Items.GetItemAt(0) as ItemPanel).Parent.Count(x => x == '.') == 0 && (myList.Items.GetItemAt(0) as ItemPanel).InFolder())
                {
                    (myList.Items.GetItemAt(0) as ItemPanel).Clear();
                    myList.Items.Clear();
                    var wr = new ItemPanel();
                    wr.DoubleClick += OpenDisk;
                    adr.SetText("C:\\");
                    myList.Items.Add(wr);
                    UserName_Changed(UserName, null);
                    FormatBtn.Visibility = Visibility.Hidden;
                }
                else if ((myList.Items.GetItemAt(0) as ItemPanel).Parent!="")
                {
                    var wr = new ItemPanel((myList.Items.GetItemAt(0) as ItemPanel).Parent.Substring(((myList.Items.GetItemAt(0) as ItemPanel).InFolder())?(myList.Items.GetItemAt(0) as ItemPanel).Parent.IndexOf(".") + 1:0), true);
                    (myList.Items.GetItemAt(0) as ItemPanel).Clear();
                    myList.Items.Clear();
                    wr.DoubleClick += OpenDisk;
                    string addr="";//="C:\\";
                    foreach (string item in wr.Parent.Split('.'))
                    {
                        addr = item + "\\" + addr;
                    }
                    adr.SetText("C:\\" + addr);
                    myList.Items.Add(wr);
                    UserName_Changed(UserName, null);
                    FormatBtn.Visibility = Visibility.Hidden;
                }

                else
                {
                    (myList.Items.GetItemAt(0) as ItemPanel).Clear();
                    myList.Items.Clear();
                    var wr = new ItemPanel();
                    wr.DoubleClick += OpenDisk;
                    adr.SetText("C:\\");
                    myList.Items.Add(wr);
                    UserName_Changed(UserName, null);
                    FormatBtn.Visibility = Visibility.Hidden;

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
                if (((ItemPanel)myList.Items.GetItemAt(0)).FolderFocused())
                {
                    string end;
                    if(((ItemPanel)myList.Items.GetItemAt(0)).Parent=="")
                        end = name;
                    else
                        end = name + "." + ((ItemPanel)myList.Items.GetItemAt(0)).Parent;
                    foreach (string item in Directory.GetFiles("../Debug/").Select(x => x.Substring(x.IndexOf('.') + 9)).Where(x => x.Substring(x.Length - end.Length) == end))
                        File.Delete(item);
                }
                else if (((ItemPanel)myList.Items.GetItemAt(0)).InFolder())
                    File.Delete(name + ".dsk." + (myList.Items.GetItemAt(0) as ItemPanel).Parent);
                else //file type
                {
                    if (UserName.GetText() == null)
                    {
                        MessageBox.Show("You need to enter your username!");
                        return;
                    }
                    ((ItemPanel)myList.Items.GetItemAt(0)).GetDisk().DelFile(name, UserName.GetText());
                }
                ((ItemPanel)myList.Items.GetItemAt(0)).Refresh();
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
                if (((ItemPanel)myList.Items.GetItemAt(0)).InFolder()&&(myList.Items.GetItemAt(0) as ItemPanel).FolderFocused())
                    OpenFolder(((ItemPanel)myList.Items.GetItemAt(0)).GetFocused());
                else if(((ItemPanel)myList.Items.GetItemAt(0)).InFolder())
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

        private void CreateFolder_Click(object sender, RoutedEventArgs e)
        {
             try
            {
            new NewFolder(OpenFolder).ShowDialog();
             }
            catch (Exception ex)
             {
                 MessageBox.Show(ex.Message);
             }
        }

        private void FormatBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (myList.Items.GetItemAt(0) as ItemPanel).Format();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
