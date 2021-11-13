
using Wssh.Entities;
using Wssh.Abilities;
using Wssh.Utilities;
using System;

namespace Wssh.Buffs
{
  /// <summary>
  /// Holy Shock proc - Lowers the cast time of your next Holy Light or Divine Light
  /// </summary>
  public class BuffInfusion : Buff
  {
    public static string NAME = "Infusion of Light";
    public static int DURATION = 15;

    /// <summary>
    /// Cast time reduction, in seconds
    /// </summary>
    public static double REDUCTION_AMOUNT = 0.75;

    public BuffInfusion(PartyMember target)
    {
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/HolyShock.png";
      DurationRemaining = DURATION;
      Target = target;
      Name = NAME;
      Description = String.Format("{0}\nLowers the cast time of your next Holy Light or Divine Light", Name);
    }

    public static bool IsInfusionAbility(Ability ability)
    {
      return (ability.Name == AbilityHolyLight.NAME || ability.Name == AbilityDivineLight.NAME);
    }

    public static double GetHasteBonus(double baseCastTime, GameState game)
    {
      double modifiedCastTime = game.ActivePlayer.HasteMultiplier() * AbilityHolyLight.CAST_TIME;
      double infusionHasteBonus = BuffInfusion.REDUCTION_AMOUNT / modifiedCastTime;

      return infusionHasteBonus;
    }
  }
}
