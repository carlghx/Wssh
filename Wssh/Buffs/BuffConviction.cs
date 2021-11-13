
using Wssh.Entities;

namespace Wssh.Buffs
{
  /// <summary>
  /// Increases healing done after a critical effect (stacking buff)
  /// </summary>
  public class BuffConviction : Buff
  {
    public static string NAME = "Conviction";
    public static int DURATION = 8;
    public static int MAX_STACKS = 3;
    public static double CONVICTION_MULTIPLIER = 0.03;

    public BuffConviction(PartyMember target)
    {
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/Conviction.png";
      DurationRemaining = DURATION;
      Target = target;
      Name = NAME;
      Description = Name + "\nIncreases healing done after a critical effect";
    }

  }
}
