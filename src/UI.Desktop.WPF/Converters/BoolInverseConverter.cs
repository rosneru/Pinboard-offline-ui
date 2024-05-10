using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UI.Desktop.WPF.Converters
{
  public class BoolInverseConverter : IValueConverter
  {
    public object Convert(object value,
                          Type targetType,
                          object parameter,
                          CultureInfo culture)
    {
      if (value == null)
      {
        throw new ArgumentNullException();
      }

      bool transformed;
      if (bool.TryParse(value.ToString(), out transformed) == false)
      {
        throw new ArgumentException("Value is not a bool.");
      }

      return !transformed;
    }

    public object ConvertBack(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
