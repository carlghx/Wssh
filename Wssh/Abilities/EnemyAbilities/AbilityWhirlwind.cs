using System;
using System.Collections.Generic;

using Wssh.Entities;

namespace Wssh.Abilities.EnemyAbilities
{
  /// <summary>
  /// Deals damage to all melee characters in group
  /// </summary>
  public class AbilityWhirlwind : EnemyAbility
  {
    public const string NAME = "Whirlwind";

    public static int CAST_TIME = 0;
    public static double COOLDOWN = 16.0;
    public static double MISS_CHANCE = 0.00;

    /// <summary>
    /// Bonus damage this ability does to non-tank melee characters
    /// </summary>
    public const double ROGUE_MULTIPLIER = 1.5;

    protected int GetDamage()
    {
      Random random = App.RandomGen;
      int damage = random.Next(BASE_MIN, BASE_MAX);

      AddDamageModifiers(ref damage);

      return damage;
    }

    public AbilityWhirlwind(GameState game) : base(game, 12000, 14000)
    {
      CastTime = CAST_TIME;
      Cooldown = COOLDOWN;
      MissChance = MISS_CHANCE;
      CooldownRemaining = Cooldown;
      UseImmediately = false;
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/Whirlwind.png";
      Name = NAME;
      TargetType = AbilityTargetType.Multiple;
    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      PutOnCooldown();

      bool targetFound = false;
      foreach (PartyMember target in targets)
      {
        if (target.Tank)
        {
          Random random = App.RandomGen;
          if (random.Next() < MissChance)
            return false;

          int damage = GetDamage();

          HandleDamage(target, damage);
          LogTargetedAttack(target, damage);

          targetFound = true;
        }
        else if (target.Melee)
        {
          Random random = App.RandomGen;
          if (random.Next() < MissChance)
            return false;

          int damage = GetDamage();
          damage = (int)(damage * ROGUE_MULTIPLIER);

          HandleDamage(target, damage);
          LogTargetedAttack(target, damage);

          targetFound = true;
        }
      }

      _game.ActiveEnemy.UpdateLastCasts(targets, IconPath, Name);
      return targetFound;
    }
  }
}
