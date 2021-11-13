using System;
using System.Collections.Generic;

using Wssh.Entities;
using Wssh.Buffs;

namespace Wssh.Abilities.EnemyAbilities
{
  /// <summary>
  /// Deals damage to current target
  /// </summary>
  public class AbilityMelee : EnemyAbility
  {
    public const string NAME = "Melee";

    public static int CAST_TIME = 0;
    public static double COOLDOWN = 2.0;
    public static double MISS_CHANCE = 0.25;

    protected int GetDamage()
    {
      Random random = App.RandomGen;

      int damage = random.Next(BASE_MIN, BASE_MAX);

      AddMeleeDamageModifiers(ref damage);

      return damage;
    }

    public AbilityMelee(GameState game) : base(game, 7000, 9000)
    {
      CastTime = CAST_TIME;
      Cooldown = COOLDOWN;
      MissChance = MISS_CHANCE;
      CooldownRemaining = Cooldown;
      UseImmediately = true;
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/Blank.png";
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
          double r = random.NextDouble();
          if (r < MissChance)
          {
            LogTargetedAttackMiss(target);
            return false;
          }

          int damage = GetDamage();

          HandleDamage(target, damage);
          _game.ActiveEnemy.UpdateLastCasts(targets, IconPath, Name);

          LogTargetedAttack(target, damage);

          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Puts enemy attack on cooldown after each use
    /// </summary>
    protected override void PutOnCooldown()
    {
      base.PutOnCooldown();

      Buff frenzy = _game.ActiveEnemy.FindBuff(BuffFrenzy.NAME);
      if (frenzy != null)
        CooldownRemaining *= BuffFrenzy.COOLDOWN_MODIFIER;
    }

  }
}
