using System;
using System.Collections.Generic;

using Wssh.Entities;

namespace Wssh.Abilities.EnemyAbilities
{
  /// <summary>
  /// Deals damage to everyone in group
  /// </summary>
  public class AbilityVolley : EnemyAbility
  {
    public const string NAME = "Volley";

    public static int CAST_TIME = 0;
    public static double COOLDOWN = 27.0;
    public static double MISS_CHANCE = 0.00;

    protected int GetDamage()
    {
      Random random = App.RandomGen;

      int damage = random.Next(BASE_MIN, BASE_MAX);

      AddDamageModifiers(ref damage);

      return damage;

    }

    public AbilityVolley(GameState game) : base(game, 9000, 10000)
    {
      CastTime = CAST_TIME;
      Cooldown = COOLDOWN;
      MissChance = MISS_CHANCE;
      CooldownRemaining = Cooldown;
      UseImmediately = false;
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/Volley.png";
      Name = NAME;
      TargetType = AbilityTargetType.All;
    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      PutOnCooldown();

      foreach (PartyMember target in targets)
      {
        Random random = App.RandomGen;
        if (random.Next() < MissChance)
          return false;

        int damage = GetDamage();

        HandleDamage(target, damage);
        LogTargetedAttack(target, damage);
      }

      _game.ActiveEnemy.UpdateLastCasts(targets, IconPath, Name);
      return true;
    }
  }
}
