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
            //לוהסיף פרטים על הדיסק וכו
            //bar.Value = (1 - (d.HowMuchEmpty() / 1020));
            //bar.ToolTip = ((1 - bar.Value) * 1020) + "B free of 1020 B";
        }
    }
}
