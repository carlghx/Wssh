using System.Collections.Generic;
using System.ComponentModel;

using Wssh.Entities;
using Wssh.Utilities;

namespace Wssh.Abilities
{
  public enum AbilityTargetType
  {
    Single,
    Multiple,
    All
  }


  /// <summary>
  /// Base class for an ability
  /// </summary>
  public abstract class Ability : INotifyPropertyChanged
  {
    protected GameState _game;

    public readonly int BASE_MIN, BASE_MAX;

    /// <summary>
    /// Path for the image file used for this ability's icon
    /// </summary>
    public string IconPath { get; set; }
    public string Name { get; set; }

    private double _castTime;
    /// <summary>
    /// Time to cast this spell (in seconds)
    /// </summary>
    public double CastTime
    {
      get
      {
        return _castTime;
      }
      set
      {
        _castTime = value;
      }
    }

    protected Ability(GameState game)
    {
      _game = game;
    }

    protected Ability(GameState game, int baseMin, int baseMax) : this(game)
    {
      BASE_MIN = baseMin;
      BASE_MAX = baseMax;
    }


    /// <summary>
    /// How long before this ability can be used again after a cast (in seconds)
    /// </summary>
    public double Cooldown { get; set; }

    public AbilityTargetType TargetType { get; set; }

    private double _cooldownRemaining;
    public static string PropertyStringCooldownRemaining = MemberNameFinder<Ability>.GetMemberName(x => x.CooldownRemaining);
    /// <summary>
    /// How long currently before this ability can be used again (in seconds)
    /// </summary>
    public double CooldownRemaining
    {
      get
      {
        return _cooldownRemaining;
      }
      set
      {
        _cooldownRemaining = value;
        OnPropertyChanged(PropertyStringCooldownRemaining);
        OnPropertyChanged("Opacity");
      }
    }



    /// <summary>
    /// All specific spells inheriting from this class should override this method with the Spell's effect
    /// </summary>
    /// <param name="targets">All players affected</param>
    /// <returns>True if cast was successful; false otherwise</returns>
    public abstract bool ExecuteCast(List<PartyMember> targets);

    protected virtual void PutOnCooldown()
    {
      CooldownRemaining += Cooldown;
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
