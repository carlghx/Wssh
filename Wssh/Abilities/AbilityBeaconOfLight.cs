using System;
using System.Collections.Generic;

using Wssh.Entities;
using Wssh.Utilities;
using Wssh.Buffs;

namespace Wssh.Abilities
{
  /// <summary>
  /// Heals directed at other targets will also heal the Beacon of Light
  /// </summary>
  public class AbilityBeaconOfLight : PlayerAbility
  {
    public const string NAME = "Beacon Of Light";
    public const string DEFAULT_KEY = "6";
    public static double HEAL_MODIFIER = 0.5;
    public const string ICON_PATH = "pack://application:,,/Wssh;component/Images/Icons/BeaconOfLight.png";
    public const int COST = 0;

    public AbilityBeaconOfLight(GameState game) : base(game)
    {
      CastTime = 0;
      ManaCost = COST;
      Cooldown = GCD;
      UsesGCD = true;
      IconPath = ICON_PATH;
      Name = NAME;
      Hotkey = HotkeyInfo.CreateFromKeyString(DEFAULT_KEY);
      Description = String.Format("{0}\nBuffs a target with a Beacon Of Light.\nAny heal you cast on other targets will also heal the Beacon Of Light for {1}% of the amount healed.", NAME, AbilityBeaconOfLight.HEAL_MODIFIER * 100);
      TargetType = AbilityTargetType.Single;
    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      if (ManaCost <= _game.ActivePlayer.ManaCurrent)
      {
        _game.ActivePlayer.ManaCurrent -= ManaCost;

        LogNonHeal();

        Buff beaconBuff = new BuffBeaconOfLight(targets[0]);
        _game.ActivePlayer.AddUniqueBuff(beaconBuff, _game);
        _game.ActivePlayer.LastCast.Update(targets, IconPath, false);
        return true;
      }

      return false;
    }

  }
}
