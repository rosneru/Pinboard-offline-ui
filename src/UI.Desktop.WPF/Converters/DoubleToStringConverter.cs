using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UI.Desktop.WPF.Converters
{
  public class DoubleToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((double)value).ToString("0.00");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string stringValue = value as string;
      if(string.IsNullOrEmpty(stringValue))
      {
        return null;
      }

      // Limit to 2 decimals
      const int numDecimals = 2;
      char decSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
      var splitComma = stringValue.Split(decSeparator);
      if(splitComma.Length > 1)
      {
        var left = splitComma[0];
        var right = splitComma[1];
        if(right.Length > numDecimals) // only 2 decimals allowed
        {
          stringValue = $"{left},{right.Substring(0, numDecimals)}";
        }
      }


      double d = 0;
      return double.TryParse(stringValue, out d) ? d : 0;
    }
  }
}
