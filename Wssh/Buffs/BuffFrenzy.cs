

namespace Wssh.Buffs
{
  /// <summary>
  /// Increases melee speed and damage for a short time
  /// </summary>
  public class BuffFrenzy : Buff
  {
    public static string NAME = "Frenzy";
    public static int DURATION = 8;

    public static double COOLDOWN_MODIFIER = 0.5;
    public static double DAMAGE_MULTIPLIER = 1.5;

    public BuffFrenzy()
    {
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/Frenzy.png";
      DurationRemaining = DURATION;
      Name = NAME;
    }
  }
}
