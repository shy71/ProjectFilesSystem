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
    /// Interaction logic for FileIcon.xaml
    /// </summary>
    public partial class FileIcon : UserControl
    {
        public FileIcon()
        {
            InitializeComponent();
        }
                /// <summary>
        /// Create a new Disk Icon
        /// </summary>
        /// <param name="d">disk needs to be already open!</param>
        public FileIcon(FMS_adapter.Disk d, string name)
        {
            InitializeComponent();
            this.name.Content = name;
            FMS_adapter.FCB fcb = d.OpenFile(name, "System", "I");
            FMS_adapter.DirEntry entry=fcb.GetDirEntry();
            bar.Minimum = 0;
            bar.Maximum = ((int)(1020 * entry.FileSize) / entry.MaxRecSize);
            bar.Value = entry.EofRecNum;
           this.ToolTip = App.NumByteToString(1020 * entry.FileSize-entry.EofRecNum * entry.MaxRecSize) + " free of " + App.NumByteToString(1020 * entry.FileSize);
            fcb.CloseFile();
        }
    }
}
