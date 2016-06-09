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
        static FMS_adapter.Disk TryOpen(string str)
        {
            try
            {
                FMS_adapter.Disk d = new FMS_adapter.Disk();
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
        /// <param name="d">disk needs to be already open!</param>
        public DiskIcon(FMS_adapter.Disk d)
        {
            InitializeComponent();
            d.GetVolumeHeader();
            name.Content=d.GetName();
            bar.Value=(1- (d.HowMuchEmpty()/1020));
            barLabel.Content = ((1 - bar.Value) * 1020) + "B free of 1020 B";
        }
    }
}
