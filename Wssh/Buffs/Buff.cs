using System;
using System.ComponentModel;

using Wssh.Entities;
using Wssh.Utilities;

namespace Wssh.Buffs
{
  /// <summary>
  /// Base class for a buff (persistent effect)
  /// </summary>
  public abstract class Buff : INotifyPropertyChanged
  {
    public const int INFINITE_DURATION = 123456;

    public bool IsDebuff { get; set; }

    public static string PropertyStringAmount = MemberNameFinder<Buff>.GetMemberName(x => x.Amount);
    private int _amount;
    /// <summary>
    /// The value (e.g. Illuminated Healing shield) or number of stacks (e.g. Conviction) associated with this buff
    /// </summary>
    public int Amount
    {
      get
      {
        return _amount;
      }
      set
      {
        _amount = value;
        OnPropertyChanged(PropertyStringAmount);
      }
    }

    public string IconPath { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public PartyMember Target { get; set; }

    public static string PropertyStringDurationRemaining = MemberNameFinder<Buff>.GetMemberName(x => x.DurationRemaining);
    private double _durationRemaining;
    /// <summary>
    /// Time before this buff expires, in seconds
    /// </summary>
    public double DurationRemaining
    {
      get
      {
        return _durationRemaining;
      }
      set
      {
        _durationRemaining = value;
        OnPropertyChanged(PropertyStringDurationRemaining);
      }
    }

    /// <summary>
    /// Creates a combat log message to be used when this buff is first created
    /// </summary>
    /// <returns>Message to be added to combat log</returns>
    public string GetLogMessage(GameState game)
    {
      string message = "";
      if (Target != null)
      {
        if (Target.Name.Equals(game.ActivePlayer.Name))
          message = String.Format("{0} gains {1}", Target.Name, Name);
        else
          message = String.Format("{0} gain {1}", game.ActivePlayer.Name, Name);

        if (Amount > 0)
          message = message + String.Format(" ({0})", Amount);
      }
      else
        message = String.Format("{0} gain {1}", game.ActivePlayer.Name, Name);

      return message;
    }


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
