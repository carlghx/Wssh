using System;

using Wssh.Entities;

namespace Wssh.Buffs
{
  /// <summary>
  /// Procs off Holy Light, Divine Light, or Flash of Light - Next holy shock will not trigger a cooldown
  /// </summary>
  public class BuffDaybreak : Buff
  {
    public static string NAME = "Daybreak";
    public static int DURATION = 12;

    public static double PROC_CHANCE = 0.2;

    public BuffDaybreak(PartyMember target)
    {
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/Daybreak.png";
      DurationRemaining = DURATION;
      Target = target;
      Name = NAME;
      Description = String.Format("{0}\nYour next holy shock will not trigger a cooldown.", Name);
    }
  }
}
