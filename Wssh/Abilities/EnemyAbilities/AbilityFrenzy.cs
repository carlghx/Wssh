using System.Collections.Generic;

using Wssh.Entities;
using Wssh.Buffs;

namespace Wssh.Abilities.EnemyAbilities
{
  /// <summary>
  /// Increases melee speed and damage for a short time
  /// </summary>
  public class AbilityFrenzy : EnemyAbility
  {
    public const string NAME = "Frenzy";

    public static int CAST_TIME = 0;
    public static double COOLDOWN = 38.0;


    public AbilityFrenzy(GameState game) : base(game)
    {
      CastTime = CAST_TIME;
      Cooldown = COOLDOWN;
      MissChance = 0;
      CooldownRemaining = Cooldown;
      UseImmediately = false;
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/Frenzy.png";
      Name = NAME;
      TargetType = AbilityTargetType.Single;
    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      PutOnCooldown();
      LogNonAttack();

      _game.ActiveEnemy.UpdateLastCasts(new List<PartyMember>(), IconPath, Name);

      BuffFrenzy buff = new BuffFrenzy();
      _game.ActiveEnemy.AddBuff(buff);

      return true;
    }
  }
}
