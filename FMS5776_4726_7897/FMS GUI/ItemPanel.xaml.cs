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

namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for ItemPanel.xaml
    /// </summary>
    public partial class ItemPanel : UserControl
    {
        bool CtrlUp=true;
        public event EventHandler OpenDiskEvent;

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
        public ItemPanel()
        {
            InitializeComponent();
            Button bt;           
            foreach (string Item in MainWindow.GetDisksNames())
            {
                bt = new Button();
                bt.BorderThickness = new Thickness(2);
                bt.Content = new DiskIcon(Item);
                bt.PreviewMouseDoubleClick += OpenDisk;
                win.Children.Add(bt);
            } 
        }
        public void Refresh()
        {
            win.Children.Clear();
            Button bt;
            foreach (string Item in MainWindow.GetDisksNames())
            {
                bt = new Button();
                bt.BorderThickness = new Thickness(2);
                bt.Content = new DiskIcon(Item);
                bt.PreviewMouseDoubleClick += OpenDisk;
                win.Children.Add(bt);
            } 
        }
        public void OpenDisk(object sender, EventArgs e)
        {
            if (OpenDiskEvent != null)
                OpenDiskEvent(sender, e);
        }
        public ItemPanel(FMS_adapter.Disk d)
        {

        }

        public string GetFocused()
        {
            foreach (Button item in win.Children)
                if (item.IsFocused)
                    return ((DiskIcon)item.Content).name.Content.ToString();
            throw new Exception("You didnt chose anything!");
        }
    }
}
