using System;
using System.Windows;

using Wssh.UI;
using Wssh.Entities;
using Wssh.Utilities;

namespace Wssh
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {

    private static Random _random;
    /// <summary>
    /// Random number generator, seeded at application start using system time
    /// </summary>
    public static Random RandomGen
    {
      get
      {
        if (_random == null)
          _random = new Random();

        return _random;
      }
    }

    public static string AppDirectory
    {
      get
      {
        return AppDomain.CurrentDomain.BaseDirectory;
      }
    }    

  }
}
