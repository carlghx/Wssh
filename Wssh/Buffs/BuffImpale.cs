using System;

using Wssh.Abilities.EnemyAbilities;
using Wssh.Entities;

namespace Wssh.Buffs
{
  public class BuffImpale : Buff, IDoT
  {
    public static string NAME = "Impale";
    public static int DURATION = 6;
    public static int TICK_INTERVAL = 2;

    /// <summary>
    /// Private constructor; use ApplyToTarget instead
    /// </summary>
    private BuffImpale()
    {
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/Impale.png";
      DurationRemaining = DURATION;
      Name = NAME;
      IsDebuff = true;
    }

    /// <summary>
    /// Create a new impale debuff on target and add it to application buff list
    /// </summary>
    /// <param name="target"></param>
    public static BuffImpale ApplyToTarget(PartyMember target, GameState _game)
    {
      BuffImpale buff = new BuffImpale() { Target = target };
      _game.ActiveEnemy.AddBuff(buff);

      return buff;
    }

    public const int BASE_MIN = 6000;
    public const int BASE_MAX = 7500;    

    #region IDoT Members    

    public int GetDamage(GameState game)
    {
      Random random = App.RandomGen;
      int damage = random.Next(BASE_MIN, BASE_MAX);

      damage = (int)(damage * game.ActiveEnemy.DamageModifier);
      return damage;
    }

    #endregion

    #region ITick Members

    /// <summary>
    /// Check to see if it's time for this DoT to tick
    /// </summary>
    /// <returns>True if it's going to tick; false otherwise</returns>
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
