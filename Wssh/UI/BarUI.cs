using System;
using System.ComponentModel;
using System.Windows.Media;

using Wssh.Utilities;
using Wssh.Entities;
using System.Windows;

namespace Wssh.UI
{
  /// <summary>
  /// UI for a display bar
  /// </summary>
  public class BarUI : INotifyPropertyChanged
  {
    public const int DEFAULT_BAR_HEIGHT = 20;

    /// <summary>
    /// Initializes a new UI for a mana bar
    /// </summary>
    /// <<param name="player">Player to create a mana bar for</param>
    /// <returns>Mana bar UI</returns>
    public static BarUI CreateManaUI()
    {
      BarUI manaUI = new BarUI();
      manaUI.Width = 200;
      manaUI.Height = DEFAULT_BAR_HEIGHT;
      manaUI.DisplayLabel = "Mana";
      manaUI.ShowValues = true;
      manaUI.ColorBrush = new SolidColorBrush(Color.FromRgb(120, 120, 255));

      return manaUI;
    }

    /// <summary>
    /// Initializes a new UI for a cast bar
    /// </summary>
    /// <returns>Cast bar UI</returns>
    public static BarUI CreateCastUI()
    {
      BarUI castUI = new BarUI();
      castUI.Width = 350;
      castUI.Height = DEFAULT_BAR_HEIGHT;
      castUI.Current = 0;
      castUI.ColorBrush = new SolidColorBrush(Color.FromRgb(255, 215, 0));
      castUI.Visible = Visibility.Collapsed;

      return castUI;
    }

    /// <summary>
    /// Initializes a new UI for a holy power bar
    /// </summary>
    /// <returns>Holy power bar UI</returns>
    public static BarUI CreateHolyUI()
    {
      BarUI holyUI = new BarUI();
      holyUI.Width = 125;
      holyUI.Height = DEFAULT_BAR_HEIGHT;
      holyUI.Current = 0;
      holyUI.Max = 3;
      holyUI.DisplayLabel = "Holy Power";
      holyUI.ShowValues = true;
      holyUI.ColorBrush = new SolidColorBrush(Color.FromRgb(238, 221, 130));

      return holyUI;
    }

    /// <summary>
    /// Whether to display Current / Max
    /// </summary>
    public bool ShowValues { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }

    public int HeightInternal
    {
      get
      {
        return Height - 2;
      }
    }

    private double _current;
    private double _max;

    private Visibility _visible;
    public static string PropertyStringVisible = MemberNameFinder<BarUI>.GetMemberName(x => x.Visible);
    public Visibility Visible
    {
      get
      {
        return _visible;
      }
      set
      {
        _visible = value;
        OnPropertyChanged(PropertyStringVisible);
      }
    }

    public static string PropertyStringCurrent = MemberNameFinder<BarUI>.GetMemberName(x => x.Current);
    /// <summary>
    /// Current value that this display bar holds
    /// </summary>
    public double Current
    {
      get
      {
        return _current;
        //return Math.Floor(_current);
      }
      set
      {
        if (value < 0)
          _current = 0;
        else if (value > Max)
          _current = Max;
        else
          _current = value;

        OnPropertyChanged(PropertyStringCurrent);
        OnPropertyChanged(PropertyStringFillWidth);
        OnPropertyChanged(PropertyStringDisplayLabel);
      }
    }

    public static string PropertyStringMax = MemberNameFinder<BarUI>.GetMemberName(x => x.Max);
    /// <summary>
    /// Maximum value that this display bar can hold
    /// </summary>
    public double Max
    {
      get
      {
        return _max;
      }
      set
      {
        _max = value;
        OnPropertyChanged(PropertyStringMax);
      }
    }

    public static string PropertyStringFillWidth = MemberNameFinder<BarUI>.GetMemberName(x => x.FillWidth);
    public int FillWidth
    {
      get
      {
        double ratio = _current / _max;

        int width = (int)(ratio * Width);
        // the full bar is slightly too large for its border
        if (ratio == 1)
          width = width - 2;

        return width;
      }

      set
      {
        int _fillWidth = value;
        double ratio = (double)_fillWidth / (double)Width;

        _current = ratio * _max;
      }
    }

    private string _label;
    public string Label
    {
      get
      {
        return _label;
      }
    }

    public static string PropertyStringDisplayLabel = MemberNameFinder<BarUI>.GetMemberName(x => x.DisplayLabel);
    /// <summary>
    /// Text label displayed over this bar
    /// </summary>
    public string DisplayLabel
    {
      get
      {
        if (ShowValues)
        {
          return String.Format("{0}: {1}/{2}", _label, Current, Max);
        }
        else
        {
          return Label;
        }

      }
      set
      {
        _label = value;
        OnPropertyChanged(PropertyStringDisplayLabel);
      }
    }

    public Brush ColorBrush { get; set; }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
      OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
    {
      var handler = PropertyChanged;
      if (handler != null)
        handler(this, args);
    }
    #endregion

  }
}
