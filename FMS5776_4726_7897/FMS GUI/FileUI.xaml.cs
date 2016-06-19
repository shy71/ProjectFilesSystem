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
            if (record == "")
                return false;
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
            if (openMode == "I")
            {
                CreateRec.Visibility = Visibility.Collapsed;
                DeleteRec.Visibility = Visibility.Collapsed;
                OpenRec.Click += OpenRecReadOnly;
            }
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
                            RecordsList.Items.Add("Record: " + s.Split('.')[0].Split(',')[1]);
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
        private void OpenRecReadOnly(object sender, RoutedEventArgs e)
        {
                string key = RecordsList.SelectedItem.ToString().Substring("Record: ".Length);
                string record;
                fcb.SeekRecord(0, 0);
                while (true)
                {
                    if (fcb.GetDirEntry().EofRecNum != fcb.GetCurrentRecordNumber())
                        fcb.ReadRecord(out record, (int)fcb.GetDirEntry().MaxRecSize, 0);
                    else
                    {
                        MessageBox.Show("The file couldn't be found...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);//to change to exp
                        return;
                    }
                    if (RecordExists(record))
                        if (Key(record) == key)
                        {
                            StringBuilder recbuilder = new StringBuilder(record);
                            new Opening_Record(ref recbuilder, (int)fcb.GetDirEntry().MaxRecSize,true).ShowDialog();
                            return;
                        }
                }
        }
        private void OpenRec_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string key = RecordsList.SelectedItem.ToString().Substring("Record: ".Length);
                string record;
                fcb.SeekRecord(0, 0);
                while (true)
                {
                    if(fcb.GetDirEntry().EofRecNum != fcb.GetCurrentRecordNumber())
                        fcb.ReadRecord(out record, (int)fcb.GetDirEntry().MaxRecSize, 1);
                    else
                    {
                        MessageBox.Show("The file couldn't be found...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (RecordExists(record) && Key(record) == key)
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
                    fcb.SeekRecord(1, 1);
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
                if (record.Length == 0)
                    return;
                string s = record.ToString() + (new string((char)0, (int)fcb.GetDirEntry().MaxRecSize - record.Length));
                fcb.AddRecord(s);
                Refresh();
            }
            catch
            {
                fcb.SeekRecord(0, 0);
            }
        }

        private void DeleteRec_Click(object sender, RoutedEventArgs e)
        {  
            if(RecordsList.SelectedItem == null)
            {
                MessageBox.Show("You haven't seleted anything to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                fcb.SeekRecord(0, 0);
                string key = RecordsList.SelectedItem.ToString().Substring("Record: ".Length), record;
                while (true)
                {
                    if (fcb.GetDirEntry().EofRecNum != fcb.GetCurrentRecordNumber())
                        fcb.ReadRecord(out record, (int)fcb.GetDirEntry().MaxRecSize, 1);
                    else
                    {
                        MessageBox.Show("The record couldn't be found...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (RecordExists(record) && Key(record) == key)
                    {
                        fcb.DeleteRecord();
                        Refresh();
                        return;
                    }
                    else
                        fcb.UpdateRecCancel();
                    fcb.SeekRecord(1, 1);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                fcb.SeekRecord(0, 0);
            }
        }

        private void RecordsList_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenRec_Click(sender, null);
        }

    }
}
