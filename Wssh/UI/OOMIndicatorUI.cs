using System.ComponentModel;

using Wssh.Abilities;
using Wssh.Utilities;
using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Wssh.UI
{
  /// <summary>
  /// Binds to SpellButton.xaml
  /// Will apply blue/black mask if ability is on cooldown or player is out of mana
  /// </summary>
  public class OOMIndicatorUI : INotifyPropertyChanged
  {
    public const double OPACITY_OOM = 0.4;
    public const double OPACITY_NORMAL = 0.0;

    private int _mana;
    public int Mana
    {
      get
      {
        return _mana;
      }
      set
      {
        _mana = value;

        OnPropertyChanged(PropertyStringHolyLight);
        OnPropertyChanged(PropertyStringDivineLight);
        OnPropertyChanged(PropertyStringFlashOfLight);
        OnPropertyChanged(PropertyStringHolyShock);
        OnPropertyChanged(PropertyStringJudgment);
        OnPropertyChanged(PropertyStringAvengingWrath);
      }
    }

    private int _holyPower;
    public int HolyPower
    {
      get
      {
        return _holyPower;
      }
      set
      {
        _holyPower = value;
        OnPropertyChanged(PropertyStringHolyPower);
      }
    }

    public static string PropertyStringHolyLight = MemberNameFinder<OOMIndicatorUI>.GetMemberName(x => x.OpacityOOMHolyLight);
    public double OpacityOOMHolyLight
    {
      get
      {
        if (Mana < AbilityHolyLight.COST)
          return OPACITY_OOM;
        else
          return OPACITY_NORMAL;
      }
    }

    public static string PropertyStringDivineLight = MemberNameFinder<OOMIndicatorUI>.GetMemberName(x => x.OpacityOOMDivineLight);
    public double OpacityOOMDivineLight
    {
      get
      {
        if (Mana < AbilityDivineLight.COST)
          return OPACITY_OOM;
        else
          return OPACITY_NORMAL;
      }
    }

    public static string PropertyStringFlashOfLight = MemberNameFinder<OOMIndicatorUI>.GetMemberName(x => x.OpacityOOMFlashOfLight);
    public double OpacityOOMFlashOfLight
    {
      get
      {
        if (Mana < AbilityFlashOfLight.COST)
          return OPACITY_OOM;
        else
          return OPACITY_NORMAL;
      }
    }

    public static string PropertyStringHolyShock = MemberNameFinder<OOMIndicatorUI>.GetMemberName(x => x.OpacityOOMHolyShock);
    public double OpacityOOMHolyShock
    {
      get
      {
        if (Mana < AbilityHolyShock.COST)
          return OPACITY_OOM;
        else
          return OPACITY_NORMAL;
      }
    }

    public static string PropertyStringAvengingWrath = MemberNameFinder<OOMIndicatorUI>.GetMemberName(x => x.OpacityOOMAvengingWrath);
    public double OpacityOOMAvengingWrath
    {
      get
      {
        if (Mana < AbilityAvengingWrath.COST)
          return OPACITY_OOM;
        else
          return OPACITY_NORMAL;
      }
    }

    public static string PropertyStringJudgment = MemberNameFinder<OOMIndicatorUI>.GetMemberName(x => x.OpacityOOMJudgment);
    public double OpacityOOMJudgment
    {
      get
      {
        if (Mana < AbilityJudgment.COST)
          return OPACITY_OOM;
        else
          return OPACITY_NORMAL;
      }
    }

    public static string PropertyStringHolyPower = MemberNameFinder<OOMIndicatorUI>.GetMemberName(x => x.OpacityOOMHolyPower);
    public double OpacityOOMHolyPower
    {
      get
      {
        if (HolyPower == 0)
          return OPACITY_OOM;
        else
          return OPACITY_NORMAL;
      }
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
