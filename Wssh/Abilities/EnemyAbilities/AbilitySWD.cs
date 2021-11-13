using System;
using System.Collections.Generic;

using Wssh.Entities;

namespace Wssh.Abilities.EnemyAbilities
{
  /// <summary>
  /// Damage to shadowpriest - will not cast if HP is low
  /// </summary>
  public class AbilitySWD : EnemyAbility
  {
    public const string NAME = "Shadow Word: Death";

    public static int CAST_TIME = 0;
    public static double COOLDOWN = 10.0;
    public static double MISS_CHANCE = 0.00;

    /// <summary>
    /// Will not cast if health is below this amount
    /// </summary>
    public static double HEALTH_THRESHOLD = 0.35;

    protected int GetDamage()
    {
      Random random = App.RandomGen;

      int damage = random.Next(BASE_MIN, BASE_MAX);

      return damage;

    }

    public AbilitySWD(GameState game) : base(game, 19000, 21000)
    {
      CastTime = CAST_TIME;
      Cooldown = COOLDOWN;
      MissChance = MISS_CHANCE;
      CooldownRemaining = Cooldown;
      UseImmediately = true;
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/SWD.png";
      Name = NAME;
      TargetType = AbilityTargetType.Single;
    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      PutOnCooldown();

      bool targetFound = false;
      foreach (PartyMember target in targets)
      {
        if (ExecuteCastOnTarget(target))
          targetFound = true;
      }

      return targetFound;
    }

    private bool ExecuteCastOnTarget(PartyMember target)
    {
      if (target.Shadow)
      {
        double healthRemaining = target.Current / target.Max;
        if (healthRemaining >= HEALTH_THRESHOLD)
        {
          Random random = App.RandomGen;
          if (random.Next() < MissChance)
            return false;

          int damage = GetDamage();

          HandleDamage(target, damage);

          LogSelfTargetedAttack(target, damage);

          return true;
        }
      }

      return false;
    }

  }  
}
