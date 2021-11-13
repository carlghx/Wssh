using System;
using System.Collections.Generic;

using Wssh.Entities;
using Wssh.Buffs;
using Wssh.Utilities;

namespace Wssh.Abilities
{
  public class AbilityHolyShock : PlayerAbility
  {
    public const string DEFAULT_KEY = "5";
    public const string NAME = "Holy Shock";
    public static int COOLDOWN = 6;
    public const int COST = 1873;

    public const double CRUSADE_MULTIPLIER = 1.3;
    
    // glyph of holy shock + infusion of light
    public const double CRIT_CHANCE_BONUS = 0.15;

    public AbilityHolyShock(GameState game) : base(game, 2629, 2847, 0.2689 * CRUSADE_MULTIPLIER)
    {
      CastTime = 0;
      ManaCost = COST;
      Cooldown = COOLDOWN;
      UsesGCD = true;
      Hotkey = HotkeyInfo.CreateFromKeyString(DEFAULT_KEY);
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/HolyShock.png";
      Name = NAME;
      Description = String.Format("{0}\n{1} mana\nInstantly heals an ally and grants a charge of Holy Power", Name, ManaCost);
      TargetType = AbilityTargetType.Single;
    }


    private int GetAmountHealed(out bool crit)
    {
      Random random = App.RandomGen;      
      crit = (random.NextDouble() <= _game.ActivePlayer.CritChance + CRIT_CHANCE_BONUS);
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

        _game.ActivePlayer.HolyPower++;
        if (crit)
        {
          Buff newBuff = new BuffInfusion(_game.ActivePlayer.PartyFrameUI);
          _game.ActivePlayer.AddOrRefreshBuff(newBuff, _game);
        }

        PutOnCooldown();

        LogTargetedHeal(target, amountHealed, crit);

        return true;
      }

      return false;
    }

    protected override void PutOnCooldown()
    {
      Buff daybreak = _game.ActivePlayer.FindBuff(BuffDaybreak.NAME);
      if (daybreak != null)
      {
        _game.ActivePlayer.RemoveBuff(daybreak, _game);
      }
      else
      {
        CooldownRemaining = COOLDOWN;
      }
    }

  }
}
