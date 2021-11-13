using System;
using System.Windows.Data;

namespace Wssh.Converters
{
  public class DoubleToTimecode : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double d = (double)value;
      int i = (int)d;

      TimeSpan ts = new TimeSpan(0, 0, i);
      return ts.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string s = value.ToString();
      return double.Parse(s);
    }

    #endregion
  }
}
