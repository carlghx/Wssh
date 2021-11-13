using System;
using System.Windows.Data;

namespace Wssh.Converters
{
  public class WidthToWidthOffset : IValueConverter
  {
    public static int OFFSET = 55;

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int i = (int)value;

      return i - OFFSET;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int i = (int)value;
      return i + OFFSET;
    }

    #endregion
  }
}
