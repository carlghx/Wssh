using System;

using Wssh.Entities;

namespace Wssh.Buffs
{
  /// <summary>
  /// Paladin Mastery ability - absorbs damage based on amount healed
  /// </summary>
  public class BuffIlluminatedHealing : Buff
  {
    public static string NAME = "Illuminated Healing";
    public static int DURATION = 8;

    public BuffIlluminatedHealing(PartyMember target, int shieldStrength)
    {
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/IlluminatedHealing.png";
      DurationRemaining = DURATION;
      Target = target;
      Amount = shieldStrength;
      Name = NAME;
      Description = String.Format("{0}\nAbsorbs damage", Name);
    }

  }
}
