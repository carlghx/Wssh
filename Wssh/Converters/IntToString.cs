using System;
using System.Windows.Data;

namespace Wssh.Converters
{
  public class IntToString : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int i = (int)value;

      if (i <= 0)
        return "";
      else
        return i.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string s = value.ToString();
      return int.Parse(s);
    }

    #endregion
  }
}
