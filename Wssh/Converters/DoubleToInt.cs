using System;
using System.Windows.Data;

namespace Wssh.Converters
{
  public class DoubleToInt : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double d = (double)value;

      return (int)d;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int i = (int)value;
      return (double)i;
    }

    #endregion
  }
}
