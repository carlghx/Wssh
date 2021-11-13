using System;

using Wssh.Entities;
using Wssh.Utilities;

namespace Wssh.Buffs
{
  public class BuffJudgment : Buff
  {
    public static string NAME = "Judgments Of The Pure";
    public static int DURATION = 60;
    public static double HASTE = 0.09;

    public BuffJudgment(PartyMember target)
    {
      Target = target;
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/JudgmentOfThePure.png";
      DurationRemaining = DURATION;
      Name = NAME;
      Description = String.Format("{0}\nIncreases casting speed", Name);
    }
  }
}
