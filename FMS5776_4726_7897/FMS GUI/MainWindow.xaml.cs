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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using FMS_adapter;

namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> disks=new List<string>();
        public MainWindow()
        {
            Button bt;
            InitializeComponent();
            foreach (string Item in Directory.GetFiles("../Debug/").Where(x => x.Substring(x.Length - 4) == ".dsk").Select(x => x.Substring(x.IndexOf('.') + 9)))
            {
                bt = new Button();
                bt.Content = new DiskIcon(Item.Substring(0, Item.IndexOf(".dsk")));
                bt.Click += OpenDisk;
                win.Children.Add(bt);
            } 
            
        }
        private void OpenDisk(object sender, RoutedEventArgs e)
        {

        }

        private void CreteDskMenu_Click(object sender, RoutedEventArgs e)
        {
            new NewDisk().ShowDialog();
        }

        private void SelectedItemProperties_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SelectedItemClose_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
