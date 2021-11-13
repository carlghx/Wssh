using System;
using System.Collections.Generic;

using Wssh.Entities;

namespace Wssh.Abilities.EnemyAbilities
{
  /// <summary>
  /// Deals damage to current target
  /// </summary>
  public class AbilitySmash : EnemyAbility
  {
    public const string NAME = "Smash";

    public static int CAST_TIME = 0;
    public static double COOLDOWN = 12.0;
    public static double MISS_CHANCE = 0.00;

    protected int GetDamage()
    {
      Random random = App.RandomGen;

      int damage = random.Next(BASE_MIN, BASE_MAX);

      AddDamageModifiers(ref damage);

      return damage;
    }

    public AbilitySmash(GameState game) : base(game, 16000, 18000)
    {
      CastTime = CAST_TIME;
      Cooldown = COOLDOWN;
      MissChance = MISS_CHANCE;
      CooldownRemaining = Cooldown;
      UseImmediately = false;
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/Smash.png";
      Name = NAME;
      TargetType = AbilityTargetType.Single;
    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      PutOnCooldown();

      foreach (PartyMember target in targets)
      {
        if (target.Tank)
        {
          Random random = App.RandomGen;
          if (random.Next() < MissChance)
            return false;

          int damage = GetDamage();

          HandleDamage(target, damage);
          _game.ActiveEnemy.UpdateLastCasts(new List<PartyMember>() { target }, IconPath, Name);

          LogTargetedAttack(target, damage);

          return true;
        }
      }

      return false;
    }
  }
}
