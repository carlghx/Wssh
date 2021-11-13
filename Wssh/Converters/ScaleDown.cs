using System;
using System.Windows.Data;

namespace Wssh.Converters
{
  public class ScaleDown : IValueConverter
  {
    double SCALE = 0.8;

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int i = (int)value;

      return (int)((double)i * SCALE);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int i = (int)value;
      return (int)((double)i / SCALE);
    }

    #endregion
  }
}
