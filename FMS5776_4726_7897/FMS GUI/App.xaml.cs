using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }
    public class EventValue : EventArgs
    {
        public object Value { get; set; }
        public string pName { get; set; }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pName"></param>
        public EventValue(object value, string pName = null)
        {
            Value = value;
            this.pName = pName;
        }
    }
}
