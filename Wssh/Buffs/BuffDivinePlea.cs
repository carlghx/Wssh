using System;

using Wssh.Entities;

namespace Wssh.Buffs
{
  public class BuffDivinePlea : Buff
  {
    public static string NAME = "Divine Plea";
    public static int DURATION = 9;
    public static double MANA_PER_TICK = 0.06;
    public static int TICK_INTERVAL = 3;
    public const double HEAL_MODIFIER = 0.5;

    public int CurrentTick { get; set; }

    public BuffDivinePlea(PartyMember target)
    {
      Target = target;
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/DivinePlea.png";
      DurationRemaining = DURATION;
      Name = NAME;
      Description = String.Format("{0}\nRestoring mana, but healing reduced", Name);
    }

    public bool CheckTick()
    {
      double currentTime = DURATION - DurationRemaining;

      if (currentTime > (CurrentTick + 1) * TICK_INTERVAL)
      {
        CurrentTick++;

        return true;
      }

      return false;
    }
  }
}
