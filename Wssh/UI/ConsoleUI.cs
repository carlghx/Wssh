using System.ComponentModel;
using System.Windows;

using Wssh.Utilities;

namespace Wssh.UI
{
  /// <summary>
  /// UI for main window
  /// </summary>
  public class ConsoleUI : INotifyPropertyChanged
  {   
    
    private bool _inCombat;
    public bool InCombat
    {
      get
      {
        return _inCombat;
      }
      set
      {
        _inCombat = value;
        OnPropertyChanged(PropertyStringCombatVisibility);
        OnPropertyChanged(PropertyStringNonCombatVisibility);
      }
    }

    public string PropertyStringCombatVisibility = MemberNameFinder<ConsoleUI>.GetMemberName(x => x.CombatVisibility);
    public Visibility CombatVisibility
    {
      get
      {
        if (_inCombat)
          return Visibility.Visible;
        else
          return Visibility.Collapsed;
      }
    }
    public string PropertyStringNonCombatVisibility = MemberNameFinder<ConsoleUI>.GetMemberName(x => x.NonCombatVisibility);
    public Visibility NonCombatVisibility
    {
      get
      {
        if (_inCombat)
          return Visibility.Collapsed;
        else
          return Visibility.Visible;
      }
    }

    public string PropertyStringElapsedTime = MemberNameFinder<ConsoleUI>.GetMemberName(x => x.ElapsedTime);
    private double _elapsedTime;
    public double ElapsedTime
    {
      get
      {
        return _elapsedTime;
      }
      set
      {
        _elapsedTime = value;
        OnPropertyChanged(PropertyStringElapsedTime);
      }
    }

    public ConsoleUI()
    {
      InCombat = false;
    }

    /// <summary>
    /// Update currently displayed time
    /// </summary>
    /// <param name="timerIncrementSeconds">Time since last update</param>
    public void Tick(double timerIncrementSeconds)
    {
      if (InCombat)
      {
        ElapsedTime += timerIncrementSeconds;
      }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;
    public virtual void OnPropertyChanged(string s)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(s));
    }
    #endregion
  }
}
