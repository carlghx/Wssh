using System;
using System.ComponentModel;

using Wssh.Abilities.EnemyAbilities;
using Wssh.Abilities;
using Wssh.Utilities;

namespace Wssh.Entities
{
  /// <summary>
  /// A list of stored game settings.
  /// Can be saved or loaded from a .wssh file using the SettingsControl
  /// </summary>
  [Serializable]
  public class Profile : INotifyPropertyChanged
  {

    #region PlayerProperties

    public static string PropertyStringName = MemberNameFinder<Profile>.GetMemberName(x => x.Name);
    private string _name;
    /// <summary>
    /// Name of this profile
    /// </summary>
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        _name = value;
      }
    }

    public static string PropertyStringIntellect = MemberNameFinder<Profile>.GetMemberName(x => x.Intellect);
    private int _intellect;
    public int Intellect 
    {
      get
      {
        return _intellect;
      }
      set
      {
        _intellect = value;
        OnPropertyChanged(PropertyStringIntellect);
      }
    }

    public static string PropertyStringSpirit = MemberNameFinder<Profile>.GetMemberName(x => x.Spirit);
    private int _spirit;
    public int Spirit 
    {
      get
      {
        return _spirit;
      }
      set
      {
        _spirit = value;
        OnPropertyChanged(PropertyStringSpirit);
      }
    }

    public static string PropertyStringCritPercent = MemberNameFinder<Profile>.GetMemberName(x => x.CritPercent);
    private int _critPercent;
    public int CritPercent 
    {
      get
      {
        return _critPercent;
      }
      set
      {
        _critPercent = value;
        OnPropertyChanged(PropertyStringCritPercent);
      }
    }

    public static string PropertyStringHastePercent = MemberNameFinder<Profile>.GetMemberName(x => x.HastePercent);
    private int _hastePercent;
    public int HastePercent 
    {
      get
      {
        return _hastePercent;
      }
      set
      {
        _hastePercent = value;
        OnPropertyChanged(PropertyStringHastePercent);
      }
    }

    public static string PropertyStringMastery = MemberNameFinder<Profile>.GetMemberName(x => x.Mastery);
    private int _mastery;
    public int Mastery 
    {
      get
      {
        return _mastery;
      }
      set
      {
        _mastery = value;
        OnPropertyChanged(PropertyStringMastery);
      }
    }

    #endregion

    #region Hotkeys

    public static string PropertyStringAW = MemberNameFinder<Profile>.GetMemberName(x => x.KeyAW);
    private string _keyAW;
    /// <summary>
    /// Hotkey string for Avenging Wrath
    /// </summary>
    public string KeyAW
    {
      get
      {
        return _keyAW;
      }
      set
      {
        _keyAW = value;
        OnPropertyChanged(PropertyStringAW);
      }
    }

    public static string PropertyStringBOL = MemberNameFinder<Profile>.GetMemberName(x => x.KeyBOL);
    private string _keyBOL;
    /// <summary>
    /// Hotkey string Beacon of Light
    /// </summary>
    public string KeyBOL
    {
      get
      {
        return _keyBOL;
      }
      set
      {
        _keyBOL = value;
        OnPropertyChanged(PropertyStringBOL);
      }
    }

    public static string PropertyStringDL = MemberNameFinder<Profile>.GetMemberName(x => x.KeyDL);
    private string _keyDL;
    /// <summary>
    /// Hotkey string for Divine Light
    /// </summary>
    public string KeyDL 
    {
      get
      {
        return _keyDL;
      }
      set
      {
        _keyDL = value;
        OnPropertyChanged(PropertyStringDL);
      }
    }

    public static string PropertyStringDP = MemberNameFinder<Profile>.GetMemberName(x => x.KeyDP);
    private string _keyDP;
    /// <summary>
    /// Hotkey string for Divine Plea
    /// </summary>
    public string KeyDP 
    {
      get
      {
        return _keyDP;
      }
      set
      {
        _keyDP = value;
        OnPropertyChanged(PropertyStringDP);
      }
    }

    public static string PropertyStringFOL = MemberNameFinder<Profile>.GetMemberName(x => x.KeyFOL);
    private string _keyFOL;
    /// <summary>
    /// Hotkey string for Flash of Light
    /// </summary>
    public string KeyFOL 
    {
      get
      {
        return _keyFOL;
      }
      set
      {
        _keyFOL = value;
        OnPropertyChanged(PropertyStringFOL);
      }
    }

    public static string PropertyStringHL = MemberNameFinder<Profile>.GetMemberName(x => x.KeyHL);
    private string _keyHL;
    /// <summary>
    /// Hotkey string for Holy Light
    /// </summary>
    public string KeyHL 
    {
      get
      {
        return _keyHL;
      }
      set
      {
        _keyHL = value;
        OnPropertyChanged(PropertyStringHL);
      }
    }

    public static string PropertyStringHS = MemberNameFinder<Profile>.GetMemberName(x => x.KeyHS);
    private string _keyHS;
    /// <summary>
    /// Hotkey string for Holy Shock
    /// </summary>
    public string KeyHS
    {
      get
      {
        return _keyHS;
      }
      set
      {
        _keyHS = value;
        OnPropertyChanged(PropertyStringHS);
      }
    }

    public static string PropertyStringJudge = MemberNameFinder<Profile>.GetMemberName(x => x.KeyJudge);
    private string _keyJudge;
    /// <summary>
    /// Hotkey string for Judgment
    /// </summary>
    public string KeyJudge
    {
      get
      {
        return _keyJudge;
      }
      set
      {
        _keyJudge = value;
        OnPropertyChanged(PropertyStringJudge);
      }
    }

    public static string PropertyStringLOD = MemberNameFinder<Profile>.GetMemberName(x => x.KeyLOD);
    private string _keyLOD;
    /// <summary>
    /// Hotkey string for Light of Dawn
    /// </summary>
    public string KeyLOD 
    {
      get
      {
        return _keyLOD;
      }
      set
      {
        _keyLOD = value;
        OnPropertyChanged(PropertyStringLOD);
      }
    }

    public static string PropertyStringWOG = MemberNameFinder<Profile>.GetMemberName(x => x.KeyWOG);
    private string _keyWOG;
    /// <summary>
    /// Hotkey string for Word of Glory
    /// </summary>
    public string KeyWOG
    {
      get
      {
        return _keyWOG;
      }
      set
      {
        _keyWOG = value;
        OnPropertyChanged(PropertyStringWOG);
      }
    }

    public static string PropertyStringCancel = MemberNameFinder<Profile>.GetMemberName(x => x.KeyCancel);
    private string _keyCancel;
    /// <summary>
    /// Hotkey string for cancel cast
    /// </summary>
    public string KeyCancel 
    {
      get
      {
        return _keyCancel;
      }
      set
      {
        _keyCancel = value;
        OnPropertyChanged(PropertyStringCancel);
      }
    }

    #endregion

    public static string PropertyStringEnemyDamage = MemberNameFinder<Profile>.GetMemberName(x => x.EnemyDamagePercent);
    private int _enemyDamagePercent;
    public int EnemyDamagePercent 
    {
      get
      {
        return _enemyDamagePercent;
      }
      set
      {
        _enemyDamagePercent = value;
        OnPropertyChanged(PropertyStringEnemyDamage);
      }
    }

    /// <summary>
    /// Private constructor; use LoadProfileFromGame instead
    /// </summary>
    private Profile()
    {
    }

    public static Profile LoadProfileFromGame(GameState game)
    {
      Profile p = new Profile();
      p.Name = "ProfileName";

      p.Intellect = game.ActivePlayer.Intellect;
      p.Spirit = game.ActivePlayer.Spirit;
      p.CritPercent = (int)(game.ActivePlayer.CritChance * 100);
      p.HastePercent = (int)(game.ActivePlayer.Haste * 100);
      p.Mastery = game.ActivePlayer.Mastery;
      p.EnemyDamagePercent = (int)(game.ActiveEnemy.DamageModifier * 100);

      System.Collections.Generic.Dictionary<Type, PlayerAbility> abilities = game.ActivePlayer.Abilities;
      p.KeyAW = abilities[typeof(AbilityAvengingWrath)].Hotkey.DisplayString;
      p.KeyBOL = abilities[typeof(AbilityBeaconOfLight)].Hotkey.DisplayString;
      p.KeyDL = abilities[typeof(AbilityDivineLight)].Hotkey.DisplayString;
      p.KeyDP = abilities[typeof(AbilityDivinePlea)].Hotkey.DisplayString;
      p.KeyFOL = abilities[typeof(AbilityFlashOfLight)].Hotkey.DisplayString;

      p.KeyHL = abilities[typeof(AbilityHolyLight)].Hotkey.DisplayString;
      p.KeyHS = abilities[typeof(AbilityHolyShock)].Hotkey.DisplayString;
      p.KeyJudge = abilities[typeof(AbilityJudgment)].Hotkey.DisplayString;
      p.KeyLOD = abilities[typeof(AbilityLightOfDawn)].Hotkey.DisplayString;
      p.KeyWOG = abilities[typeof(AbilityWordOfGlory)].Hotkey.DisplayString;

      p.KeyCancel = game.CancelKey.DisplayString;

      return p;
    }

    #region INotifyPropertyChanged Members
    
    [field:NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
      if(PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}
