
using Wssh.Entities;
using Wssh.Utilities;
using System;

namespace Wssh.Buffs
{
  /// <summary>
  /// Increases all healing done for a short time
  /// </summary>
  public class BuffAvengingWrath : Buff
  {
    public static string NAME = "Avenging Wrath";
    public static int DURATION = 20;
    public static double MODIFIER = 1.2;

    public BuffAvengingWrath(PartyMember target)
    {
      Target = target;
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/AvengingWrath.png";
      DurationRemaining = DURATION;
      Name = NAME;
      Description = String.Format("{0}\nIncreases all healing done by {1}% for {2} seconds.", Name, (BuffAvengingWrath.MODIFIER - 1) * 100, BuffAvengingWrath.DURATION);
    }
  }
}
