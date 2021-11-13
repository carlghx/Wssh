using System;
using System.Windows.Data;

namespace Wssh.Converters
{
  public class StringToFontSize : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string s = value.ToString();

      if (s.Length < 4)
        return 32;
      else if (s.Length < 12)
        return 14;
      else
        return 10;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return "";
    }

    #endregion
  }
}
