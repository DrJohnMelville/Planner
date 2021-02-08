using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Planner.Wpf.TaskList
{
    public class AndVisibilityConverter: IMultiValueConverter
    {
        public static readonly AndVisibilityConverter Instance = new AndVisibilityConverter(); 
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => 
            values?.All(i => i is bool b && b)??false ? Visibility.Visible : Visibility.Collapsed;

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}