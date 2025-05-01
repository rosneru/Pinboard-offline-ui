using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace UI.Desktop.WPF.Converters
{
  class BackgroundColorConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length < 2)
      {
        return Brushes.Transparent;
      }

      var item = values[0];
      var highlightList = values[1] as System.Collections.IEnumerable;

      if (highlightList != null && highlightList.Cast<object>().Contains(item))
      {
        return Brushes.IndianRed;
      }

      return Brushes.DimGray;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
