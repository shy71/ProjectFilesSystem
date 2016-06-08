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
using System.Windows.Shapes;
using FMS_adapter;
namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for ShowDisk.xaml
    /// </summary>
    public partial class ShowDisk : Window
    {
        VolumeHeader vh;
        public ShowDisk()
        {
            InitializeComponent();
        }
        public ShowDisk(Disk d)
        {
            InitializeComponent();
            vh=d.GetVolumeHeader();
            d.g
        }
    }
}
