using System;


namespace Wssh.Buffs
{
  public class BuffVampiricEmbrace : Buff, IHoT
  {
    public static string NAME = "Vampiric Embrace";
    public static int DURATION = Buff.INFINITE_DURATION;
    public static int TICK_INTERVAL = 1;

    public int CurrentTick { get; set; }

    public BuffVampiricEmbrace()
    {
      IconPath = "";
      DurationRemaining = DURATION;
      Name = NAME;
      IsDebuff = true;
    }

    public const int BASE_MIN = 1900;
    public const int BASE_MAX = 2100;

    #region IHoT Members

    public int GetHeal()
    {
      Random random = App.RandomGen;
      int heal = random.Next(1900, 2100);
      return heal;
    }

    #endregion

    #region ITick Members

    public bool CheckTick()
    {

      if (Timer >= TICK_INTERVAL)
      {
        Timer = 0;
        return true;
      }

      return false;
    }

    public double Timer { get; set; }

    #endregion
  }
}
