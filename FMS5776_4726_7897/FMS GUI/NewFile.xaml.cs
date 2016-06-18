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

namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for NewFile.xaml
    /// </summary>
    public partial class NewFile : Window
    {
        FMS_adapter.Disk d;
        public NewFile(FMS_adapter.Disk d)
        {
            InitializeComponent();
            this.d = d;
        }

        private void Create_Click(object sender, RoutedEventArgs e) 
        {
            try
            {
                d.CreateFile(name.GetText(), owner.GetText(), "F", uint.Parse(RecordSize.GetText()), ((uint)(uint.Parse(FileSize.GetText()) / 1020)) + 1, KeyType.GetText(), 0);
                this.Close();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message, "Error: Create Disk", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
        }
    }
}
