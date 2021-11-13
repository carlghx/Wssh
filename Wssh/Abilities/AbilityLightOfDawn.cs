using System;
using System.Collections.Generic;

using Wssh.Entities;
using Wssh.Utilities;

namespace Wssh.Abilities
{
  /// <summary>
  /// Consumes all Holy Power to heal all friendly targets
  /// </summary>
  public class AbilityLightOfDawn : PlayerAbility
  {
    public const string NAME = "Light Of Dawn";
    public const string DEFAULT_KEY = "H";
    public static int CAST_TIME = 0;

    public AbilityLightOfDawn(GameState game) : base(game, 605, 673, 0.132)
    {
      CastTime = CAST_TIME;
      ManaCost = 0;
      Cooldown = GCD;
      UsesHolyPower = true;
      UsesGCD = true;
      Hotkey = HotkeyInfo.CreateFromKeyString(DEFAULT_KEY);
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/LightOfDawn.png";
      Name = NAME;
      Description = String.Format("{0}\nConsumes all Holy Power to heal all friendly targets.", Name);
      TargetType = AbilityTargetType.All;
    }

    private int GetAmountHealed(int holyPower, out bool crit, PartyMember target)
    {
      Random random = App.RandomGen;

      crit = (random.NextDouble() <= _game.ActivePlayer.CritChance);

      int baseAmount = random.Next(BASE_MIN, BASE_MAX);
      double bonus = _game.ActivePlayer.SpellPower * POWER_MULTIPLIER;

      double baseHeal = baseAmount + bonus;
      baseHeal *= holyPower;
      int modifiedHeal = AddHealModifiers(crit, baseHeal);

      return modifiedHeal;

    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      if (ManaCost <= _game.ActivePlayer.ManaCurrent && _game.ActivePlayer.HolyPower > 0)
      {
        _game.ActivePlayer.ManaCurrent -= ManaCost;

        foreach (PartyMember target in targets)
        {
          HealPlayer(target);
        }

        _game.ActivePlayer.LastCast.Update(targets, IconPath, false);
        _game.ActivePlayer.HolyPower = 0;
        return true;
      }

      return false;
    }

    private void HealPlayer(PartyMember target)
    {
      bool crit;
      int amountHealed = GetAmountHealed(_game.ActivePlayer.HolyPower, out crit, target);

      LogTargetedHeal(target, amountHealed, crit);

      HandleAOEHeal(target, amountHealed, crit);
    }
  }
}
