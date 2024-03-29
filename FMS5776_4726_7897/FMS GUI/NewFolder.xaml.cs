﻿using System;
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
    public partial class NewFolder : Window
    {
        Action<string> func;

        public NewFolder(Action<string> func)
        {
            InitializeComponent();
            this.func = func;
        }

        private void Create_Click(object sender, RoutedEventArgs e) 
        {
            try
            {
                if (name.GetText().Contains(' '))
                    throw new Exception("You cant use spaces");
                func(name.GetText());
                this.Close();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
        }
    }
}
