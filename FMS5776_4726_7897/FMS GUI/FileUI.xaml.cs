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
            return record.Split('.')[0].Split(',')[1];
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
                RecordsList.Items.Clear();
                //List<string> recordlist = new List<string>();
                while (true)
                {
                    string s;
                    if (fcb.GetCurrentRecordNumber() != fcb.GetDirEntry().EofRecNum)
                    {
                        fcb.ReadRecord(out s, (int)fcb.GetDirEntry().MaxRecSize);
                        if (RecordExists(s))
                        {
                            //recordlist.Add(s);
                            RecordsList.Items.Add(s.Split('.')[0].Split(',')[1]);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                fcb.SeekRecord(0, 0);
            }
            catch
            {
                fcb.SeekRecord(0, 0);
            }
        }
        private void OpenRec_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string key = (string)RecordsList.SelectedItem;
                fcb.SeekRecord(0, 0);
                while (true)
                {
                    string record;
                    if(fcb.GetDirEntry().EofRecNum != fcb.GetCurrentRecordNumber())
                        fcb.ReadRecord(out record, (int)fcb.GetDirEntry().MaxRecSize, 1);
                    else
                    {
                        MessageBox.Show("The file couldn't be found...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (Key(record) == key)
                    {
                        StringBuilder recbuilder = new StringBuilder(record);
                        new Opening_Record(ref recbuilder, (int)fcb.GetDirEntry().MaxRecSize).ShowDialog();
                        if (recbuilder.ToString() != record)
                        {
                            if (recbuilder.Length < fcb.GetDirEntry().MaxRecSize)
                            {
                                recbuilder.Append(new string((char)0, (int)(fcb.GetDirEntry().MaxRecSize - recbuilder.Length)));
                            }
                            //fcb.UpdateRecord(recbuilder.ToString());
                            fcb.UpdateRecCancel();
                            fcb.WriteRecord(recbuilder.ToString());//update the record to its new version
                        }
                        else
                            fcb.UpdateRecCancel();
                        return;
                    }
                    else
                        fcb.UpdateRecCancel();
                }
            }
            catch
            {
                fcb.SeekRecord(0, 0);
            }
        }
        private void CreateRec_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder record = new StringBuilder();
                new Create_Record(ref record, fcb.GetDirEntry()).ShowDialog();
                string s = record.ToString() + (new string((char)0, (int)fcb.GetDirEntry().MaxRecSize - record.Length));
                fcb.AddRecord(s);
                Refresh();
            }
            catch
            {
                fcb.SeekRecord(0, 0);
            }
        }

    }
}
