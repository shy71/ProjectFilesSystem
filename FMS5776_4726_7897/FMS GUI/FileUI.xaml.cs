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
        public string Key(string record)
        {
            return record.Substring((int)fcb.GetDirEntry().KeyOffset, (int)fcb.GetDirEntry().KeySize);
        }
        public bool RecordExists(string record)
        {
            foreach (var c in Key(record))
                if (c != null)
                    return true;
            return false;
        }
        public FileUI(Disk d,string name, string owner,string openMode)
        {
            InitializeComponent();
            fcb = d.OpenFile(name, owner, openMode);
            Refresh();
        }
        private void Refresh()
        {
            try
            {
                fcb.SeekRecord(0, 0);
                List<string> recordlist = new List<string>();
                while (true)
                {
                    string s;
                    fcb.ReadRecord(out s, (int)fcb.GetDirEntry().MaxRecSize);
                    if (RecordExists(s))
                    {
                        recordlist.Add(s);
                        RecordsList.Items.Add(s.Substring((int)fcb.GetDirEntry().KeyOffset, (int)fcb.GetDirEntry().KeySize));
                    }
                }
            }
            catch
            {
                fcb.SeekRecord(0, 0);
            }
        }
        private void OpenRec_Click(object sender, RoutedEventArgs e)
        {
            string key = (string)RecordsList.SelectedItem;
        }
        private void CreateRec_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder record = new StringBuilder();
            new Create_Record(ref record,fcb.GetDirEntry()).ShowDialog();
            string s = record.ToString() + (new string((char)0, (int)fcb.GetDirEntry().MaxRecSize - record.Length));
            string currec;
            fcb.SeekRecord(0, 0);
            fcb.ReadRecord(out currec,(int)fcb.GetDirEntry().MaxRecSize);
            while (RecordExists(currec));
            fcb.SeekRecord(0, -1);
            fcb.WriteRecord(s);
            Refresh();
        }

        private void RecProprerties_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
