using System;
using System.Collections.Generic;

using Wssh.Entities;
using Wssh.Buffs;

namespace Wssh.Abilities.EnemyAbilities
{
  /// <summary>
  /// Does initial damage, then damage over time to a single target
  /// </summary>
  public class AbilityImpale : EnemyAbility
  {
    public const string NAME = "Impale";

    public static int CAST_TIME = 0;
    public static double COOLDOWN = 9.0;
    public static double MISS_CHANCE = 0.00;

    /// <summary>
    /// Get initial damage done; DoT damage comes from BuffImpale
    /// </summary>
    /// <returns>Initial damage done by that ability</returns>
    protected int GetDamage()
    {     
      int damage = App.RandomGen.Next(BASE_MIN, BASE_MAX);

      AddDamageModifiers(ref damage);

      return damage;
    }

    public AbilityImpale(GameState game) : base(game, 1000, 3000)
    {
      CastTime = CAST_TIME;
      Cooldown = COOLDOWN;
      MissChance = MISS_CHANCE;
      CooldownRemaining = Cooldown;
      UseImmediately = false;
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/Impale.png";
      Name = NAME;
      TargetType = AbilityTargetType.Single;
    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      PutOnCooldown();

      Random random = App.RandomGen;
      if (random.Next() < MissChance)
        return false;

      int targetIndex = random.Next(targets.Count);

      return ExecuteCastOnTarget(targets[targetIndex]);
    }

    private bool ExecuteCastOnTarget(PartyMember target)
    {
      int damage = GetDamage();

      HandleDamage(target, damage);
      target.StatusDebuff = IconPath;
      _game.ActiveEnemy.UpdateLastCasts(new List<PartyMember>() { target }, IconPath, Name);

      LogTargetedAttack(target, damage);
      BuffImpale.ApplyToTarget(target, _game);

      return true;
    }

  }
}
