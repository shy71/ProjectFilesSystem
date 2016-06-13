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
    /// Interaction logic for FileUI.xaml
    /// </summary>
    public partial class FileUI : Window
    {
        FCB fcb;
        public FileUI()
        {
            InitializeComponent();
        }
        public FileUI(Disk d,string name, string owner,string openMode)
        {
            InitializeComponent();
            fcb = d.OpenFile(name, owner, openMode);
        }

        private void OpenRec_Click(object sender, RoutedEventArgs e)
        {
            while(true)
            {
                
                string s = "";
                fcb.ReadRecord(ref s);
               // if(fcb.
            }
        }
    }
}
