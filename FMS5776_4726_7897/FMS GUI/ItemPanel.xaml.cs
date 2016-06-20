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
using System.ComponentModel;
using System.IO;
using FMS_adapter;

namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for ItemPanel.xaml
    /// </summary>
    public partial class ItemPanel : UserControl
    {
        Disk d = null;
        private string username;
            
        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        private string parent="";

        public string Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        public event EventHandler DoubleClick;

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        //}
        //protected bool SetField<T>(ref T field, T value, string propertyName)
        //{
        //    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        //    field = value;
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}
        public void Clear()
        {
            if (d != null)
                d.UnmountDisk();
        }
        ~ItemPanel()
        {
            Clear();
        }
        public bool InFolder()
        {
            return d == null;
        }
        public Disk GetDisk()
        {
            if (d != null)
                return d;
            throw new Exception("You are not Inside a disk!");
        }
        public ItemPanel()
        {
            InitializeComponent();
            Refresh();
        }
        /// <summary>
        /// refreshes the panel
        /// </summary>
        /// <param name="ParentSub"></param>
        public void Refresh(string ParentSub="")
        {
            ParentSub = Parent;
            if (d == null)
            {
                win.Children.Clear();
                Button bt;
                foreach (string Item in MainWindow.GetDisksNames(ParentSub))
                {
                    bt = new Button();
                    bt.ToolTip = "Disk";
                    bt.Content = new DiskIcon(Item,ParentSub);
                    bt.Name = Item;
                    bt.PreviewMouseDoubleClick += DoubleClickEvent;
                    win.Children.Add(bt);
                }
                foreach (string Item in MainWindow.GetFolderNames(ParentSub))
                {
                    bt = new Button();
                    bt.ToolTip = "Folder";
                    bt.Content = new FolderIcon(Item);
                    bt.Name = Item;
                    bt.PreviewMouseDoubleClick += DoubleClickEvent;
                    win.Children.Add(bt);
                }
            }
            else
            {
                win.Children.Clear();
                var names = d.GetFilesNames();
                Button bt;
                foreach (string item in names)
                {
                    bt = new Button();
                    bt.Content = new FileIcon(d, item);
                    bt.Name = item;
                    bt.PreviewMouseDoubleClick += OpenFile;
                    win.Children.Add(bt);
                }
            }
        }
        public void OpenFile(object sender, EventArgs e)
        {
            OpenFile((sender as Button).Name);
        }
        /// <summary>
        /// opens a file
        /// </summary>
        /// <param name="name"></param>
        public void OpenFile(string name)
        {
            try
            {
                bool ReadOnly = false;
                if (MessageBox.Show("Do you want to open it in read-only mode?", "Open Mode", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    ReadOnly = true;
                if(!ReadOnly)
                foreach (Window item in App.Current.Windows)
                    if (item.GetType() == typeof(FileUI))
                        if ((item as FileUI).GetFileName() == name)
                            if (!(item as FileUI).ReadOnly)
                                throw new Exception("You can't open a file in wirting mode more then once!");
                if (Username == null)
                    throw new Exception("You need to enter your username!");
                if (ReadOnly)
                    new FileUI(d, name, Username, "I").Show();
                else
                    new FileUI(d, name, Username, "IO").Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void DoubleClickEvent(object sender, EventArgs e)
        {
            if (DoubleClick != null)
                DoubleClick(sender, e);
        }
        public ItemPanel(string DiskName,string sub)
        {
            Parent = sub;
            if (sub == "")
                sub = ".dsk";
            else
                sub = ".dsk." + sub;

            InitializeComponent();
            d = new Disk();
            d.SetEnd(sub);
            d.MountDisk(DiskName);
            Refresh();
           
        }
        public ItemPanel(string FolderName,bool IsFolder)
        {
            Parent = FolderName;
            InitializeComponent();
            Refresh();

        }
        public void CreateFile(bool Create=true)
        {
            if (d == null)
                throw new Exception("You can't create a file outside of a disk");
            if (Create)
            {
                new NewFile(d).ShowDialog();
                Refresh();
            }
        }
        /// <summary>
        /// the selected item is a folder
        /// </summary>
        /// <returns></returns>
        public bool FolderFocused()
        {
            foreach (Button item in win.Children)
                if (item.IsFocused)
                {
                    if (item.Content.GetType() == typeof(FileIcon))
                        return false;
                    else if (item.Content.GetType() == typeof(FolderIcon))
                        return true;
                    return false;
                }
            throw new Exception("You didnt choose anything!");
        }
        /// <summary>
        /// returns the name of the selected item
        /// </summary>
        /// <returns></returns>
        public string GetFocused()
        {
            foreach (Button item in win.Children)
                if (item.IsFocused)
                {
                    if (item.Content.GetType() == typeof(FileIcon))
                        return ((FileIcon)item.Content).name.Content.ToString();
                    else if (item.Content.GetType() == typeof(FolderIcon))
                        return ((FolderIcon)item.Content).name.Content.ToString();
                    return ((DiskIcon)item.Content).name.Content.ToString();
                }
            throw new Exception("You didnt choose anything!");
        }
        /// <summary>
        /// formats the disk
        /// </summary>
        public void Format()
        {
            if (d == null)
                throw new Exception("you can't format outside of a disk!");
            if (Username == null)
                throw new Exception("You need to enter your username!");
            d.Format(Username);
            MessageBox.Show("The disk " + d.GetName() + " was format by " + Username);
            Refresh();
        }
        /// <summary>
        /// extends the file by a specific size
        /// </summary>
        /// <param name="numKB"></param>
        public void Extend(int numKB=10)
        {
            if (d == null)
                throw new Exception("you can't extend file outside of a disk!");
            if (Username == null)
                throw new Exception("You need to enter your username!");
            d.ExtendFile(GetFocused(), Username,(uint)numKB);
            MessageBox.Show("The file " + GetFocused() + " was extended!");
            Refresh();

        }
        /// <summary>
        /// goes out of the current level if possible
        /// </summary>
        public void Up()
        {
            if(d==null)
                throw new Exception("You can't go higher! you are already in root level!");
            else
            {
                d.UnmountDisk();
                d = null;
                Refresh();
            }
        }
    }
}
