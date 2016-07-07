using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
namespace FMS_GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string NumByteToString(float num)
        {
            if(num<1000)
                return Math.Round(num,3)+"B";
            if(num<1000000)
                return Math.Round(num/1000,2) +"KB";
            else
                return Math.Round(num / 1000000,2) + "MB";
        }
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
    public class NotVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((Visibility)value) == Visibility.Visible)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }
}
