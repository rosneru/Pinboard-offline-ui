using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace UI.Desktop.WPF.Converters
{
  public class BoolVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, 
                          object parameter, 
                          CultureInfo culture)
    {
      if(value is Boolean && ((bool)value == true))
      {
        return Visibility.Visible;
      }

      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, 
                              Type targetType, 
                              object parameter, CultureInfo culture)
    {
      if(value is Visibility && ((Visibility)value) == Visibility.Visible)
      {
        return true;
      }

      return false;
    }
  }
}
