﻿using System;
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
        Disk d = null;//לדאוג לסגור
        private string username;
            
        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        
        public event EventHandler DoubleClick;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        public bool RootLevel()
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
        public void Refresh()
        {
            if (d == null)
            {
                win.Children.Clear();
                Button bt;
                foreach (string Item in MainWindow.GetDisksNames())
                {
                    bt = new Button();
                    bt.Content = new DiskIcon(Item);
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
        public void OpenFile(string name)
        {
            try
            {
            if(Username==null)
                throw new Exception("You need to enter your username!");
            new FileUI(d, name, Username, "IO").ShowDialog();//fix for any owner
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
        public ItemPanel(string DiskName)
        {
            InitializeComponent();
            d = new Disk();
            d.MountDisk(DiskName);
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
        public string GetFocused()
        {
            foreach (Button item in win.Children)
                if (item.IsFocused)
                {
                    if (item.Content.GetType() == typeof(FileIcon))
                        return ((FileIcon)item.Content).name.Content.ToString();
                    return ((DiskIcon)item.Content).name.Content.ToString();
                }
            throw new Exception("You didnt choose anything!");
        }
        public void Up()
        {
            if(d==null)
                throw new Exception("You cant go higher! you are already in root level!");
            else
            {
                d.UnmountDisk();
                d = null;
                Refresh();
            }
        }
    }
}
