using System;
using System.Collections.Generic;

using Wssh.Entities;
using Wssh.Utilities;

namespace Wssh.Abilities
{
  public class AbilityFlashOfLight : PlayerAbility
  {
    public const string NAME = "Flash Of Light";
    public const string DEFAULT_KEY = "2";
    public static double CAST_TIME = 1.5;
    public static int COST = 7026;

    public AbilityFlashOfLight(GameState game) : base(game, 6908, 7749, 0.8632)
    {
      CastTime = CAST_TIME;
      ManaCost = COST;
      Cooldown = GCD;
      UsesGCD = true;
      Hotkey = HotkeyInfo.CreateFromKeyString(DEFAULT_KEY);
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/FlashOfLight.png";
      Name = NAME;
      Description = String.Format("{0}\n{1} mana\nA quick, expensive heal on a single target.", Name, ManaCost);
      TargetType = AbilityTargetType.Single;
    }

    private int GetAmountHealed(out bool crit)
    {
      Random random = App.RandomGen;
      crit = (random.NextDouble() <= _game.ActivePlayer.CritChance);

      int baseAmount = random.Next(BASE_MIN, BASE_MAX);
      double bonus = _game.ActivePlayer.SpellPower * POWER_MULTIPLIER;

      double baseHeal = baseAmount + bonus;

      int modifiedHeal = AddHealModifiers(crit, baseHeal);

      return modifiedHeal;

    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      if (ManaCost <= _game.ActivePlayer.ManaCurrent)
      {
        _game.ActivePlayer.ManaCurrent -= ManaCost;
        bool crit;
        int amountHealed = GetAmountHealed(out crit);
        PartyMember target = targets[0];

        HandleTargetedHeal(target, amountHealed, crit);
        _game.ActivePlayer.LastCast.Update(targets, IconPath, crit);

        LogTargetedHeal(target, amountHealed, crit);

        CheckProcDaybreak();        
        CheckTowerOfRadiance(target);

        return true;
      }

      return false;
    }

  }
}
