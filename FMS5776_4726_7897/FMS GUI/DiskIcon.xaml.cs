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
    /// Interaction logic for DiskIcon.xaml
    /// </summary>
    public partial class DiskIcon : UserControl
    {
        //add press event
        public DiskIcon()
        {
            InitializeComponent();
        }
        public DiskIcon(string str) : this(TryOpen(str)) { }
        public DiskIcon(string str,string sub) : this(TryOpen(str,sub)) { }
        static FMS_adapter.Disk TryOpen(string str,string sub="")
        {
            if (sub == "")
                sub = ".dsk";
            else
                sub = ".dsk." + sub;
            try
            {
                FMS_adapter.Disk d = new FMS_adapter.Disk();
                d.SetEnd(sub);
                d.MountDisk(str);
                return d;
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message, "Errot: Mount Disk", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        /// <summary>
        /// Create a new Disk Icon
        /// </summary>
        /// <param name="d">disk needs to be already open! and will be closed at the end</param>
        public DiskIcon(FMS_adapter.Disk d)
        {
            InitializeComponent();
            d.GetVolumeHeader();
            name.Content=d.GetName();
            bar.Value=1600-d.HowMuchEmpty();
            barLabel.Content = App.NumByteToString(d.HowMuchEmpty()*2 * 1020) +" free of " + App.NumByteToString(1024*3200);
            d.UnmountDisk();
        }
    }
}
