using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FMS_adapter;

namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }
    public class FileSys
    {
        List<File> list;
        
        void AddFile()
        {
            
        }
    }
   public interface File
   {
       string name;
       bool IsDisk;
   }
    public class DiskFile :File
    {

    }
    public class DirFile : File
    {
        List<File> list;
    }
}
