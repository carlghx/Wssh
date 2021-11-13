using System;
using System.Collections.Generic;

using Wssh.Entities;
using Wssh.Utilities;

namespace Wssh.Abilities
{
  public class AbilityWordOfGlory : PlayerAbility
  {
    public const string DEFAULT_KEY = "Q";
    public const string NAME = "Word Of Glory";
    public static int CAST_TIME = 0;

    public const double LAST_WORD_THRESHOLD = 0.35;
    public const double LAST_WORD_CRIT_CHANCE_BONUS = 0.6;

    public AbilityWordOfGlory(GameState game) : base(game, 2018, 2248, 0.198)
    {
      _game = game;

      CastTime = CAST_TIME;
      ManaCost = 0;
      Cooldown = GCD;
      UsesHolyPower = true;
      UsesGCD = true;
      Hotkey = HotkeyInfo.CreateFromKeyString(DEFAULT_KEY);
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/WordOfGlory.png";
      Name = NAME;
      Description = String.Format("{0}\nConsumes all Holy Power to heal a friendly target", Name);
      TargetType = AbilityTargetType.Single;
    }

    private int GetAmountHealed(out bool crit, PartyMember target)
    {
      Random random = App.RandomGen;

      // Last Word talent (2/2)
      double modifiedCritChance = _game.ActivePlayer.CritChance;
      if ((target.Current / target.Max) <= LAST_WORD_THRESHOLD)
        modifiedCritChance += LAST_WORD_CRIT_CHANCE_BONUS;

      crit = (random.NextDouble() <= modifiedCritChance);

      int baseAmount = random.Next(BASE_MIN, BASE_MAX);
      double bonus = _game.ActivePlayer.SpellPower * POWER_MULTIPLIER;

      double baseHeal = baseAmount + bonus;
      baseHeal *= _game.ActivePlayer.HolyPower;
      int modifiedHeal = AddHealModifiers(crit, baseHeal);

      return modifiedHeal;
    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      if (ManaCost <= _game.ActivePlayer.ManaCurrent && _game.ActivePlayer.HolyPower > 0)
      {
        _game.ActivePlayer.ManaCurrent -= ManaCost;
        bool crit;
        PartyMember target = targets[0];
        int amountHealed = GetAmountHealed(out crit, target);

        HandleTargetedHeal(target, amountHealed, crit);
        _game.ActivePlayer.LastCast.Update(targets, IconPath, crit);        

        LogTargetedHeal(target, amountHealed, crit);
        _game.ActivePlayer.HolyPower = 0;

        CheckProcDaybreak();
        
        return true;
      }
      
      return false;
    }

  }
}
