
using Wssh.Entities;
using Wssh.Buffs;
using System;

namespace Wssh.Abilities.EnemyAbilities
{

  /// <summary>
  /// Base class for an enemy ability
  /// </summary>
  public abstract class EnemyAbility : Ability
  {
    /// <summary>
    /// % chance that this ability will miss
    /// </summary>
    public double MissChance { get; set; }

    /// <summary>
    /// Whether to use this as soon as it's available, or wait a random amount of time first
    /// </summary>
    public bool UseImmediately { get; set; }

    /// <summary>
    /// How long the cooldown can vary by if USE_IMMEDIATELY is false (in seconds)
    /// </summary>
    public static int USE_DELAY_RANGE = 4;

    protected EnemyAbility(GameState game) : base(game)
    {

    }

    protected EnemyAbility(GameState game, int baseMin, int baseMax)
      : base(game, baseMin, baseMax)
    {

    }

    public static void HandleDamage(PartyMember target, int damage, GameState game)
    {
      int modifiedDamage = damage;

      // checks for Mastery shield before applying damage
      Buff mastery = game.ActivePlayer.FindBuffOnTarget(BuffIlluminatedHealing.NAME, target);
      if (mastery != null)
      {
        modifiedDamage -= mastery.Amount;
        mastery.Amount -= damage;
        if (mastery.Amount <= 0)
          game.ActivePlayer.RemoveBuff(mastery, game);
      }

      if (modifiedDamage > 0)
        target.Current -= modifiedDamage;
    }

    protected void HandleDamage(PartyMember target, int damage)
    {
      HandleDamage(target, damage, _game);
    }

    protected void AddDamageModifiers(ref int damage)
    {
      damage = (int)(damage * _game.ActiveEnemy.DamageModifier);
    }


    protected void AddMeleeDamageModifiers(ref int damage)
    {
      Buff frenzy = _game.ActiveEnemy.FindBuff(BuffFrenzy.NAME);
      if (frenzy != null)
        damage = (int)(damage * BuffFrenzy.DAMAGE_MULTIPLIER);

      damage = (int)(damage * _game.ActiveEnemy.DamageModifier);
    }


    /// <summary>
    /// Puts enemy attack on cooldown after each cast
    /// </summary>
    protected override void PutOnCooldown()
    {
      base.PutOnCooldown();
      
      if (!UseImmediately)
      {
        Random random = App.RandomGen;
        double delay = random.Next(0, EnemyAbility.USE_DELAY_RANGE);
        CooldownRemaining += delay;
      }

    }

    protected void LogTargetedAttack(PartyMember target, int damage, bool miss = false)
    {
      string message = String.Format("{0} {1} {2} {3}", _game.ActiveEnemy.Name, Name, target.Name, damage);
      if (miss)
        message = message + " (Miss)";

      _game.LogMessage(message);
    }

    protected void LogTargetedAttackMiss(PartyMember target)
    {
      string message = String.Format("{0} {1} {2} (Miss)", _game.ActiveEnemy.Name, Name, target.Name);
      _game.LogMessage(message);
    }

    protected void LogSelfTargetedAttack(PartyMember target, int damage, bool miss = false)
    {
      string message = String.Format("{1} {0} {1} {2}", Name, target.Name, damage);
      if (miss)
        message = message + " (Miss)";

      _game.LogMessage(message);
    }

    protected void LogNonAttack()
    {
      string message = String.Format("{0} {1}", _game.ActiveEnemy.Name, Name);
      _game.LogMessage(message);
    }

  }

}
