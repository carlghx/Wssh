using System.Collections.Generic;

using NUnit.Framework;
using Wssh.Entities;
using Wssh.Abilities.EnemyAbilities;
using Wssh.Buffs;

namespace WsshTest
{
  [TestFixture]
  [Description("Tests relating to enemy buffs and abilities")]
  public class TestEnemyCast
  {
    GameState game;
    PartyMember tank, shadowPriest, rogue;

    [TestFixtureSetUp]
    public void TestGame()
    {
      game = new GameState();
      game.StartCombatGame();
      game.StopUpdateTimer();
      tank = game.ActiveParty.PartyMembers.Find(x => x.Name == PartyMember.WARRIOR_NAME);
      shadowPriest = game.ActiveParty.PartyMembers.Find(x => x.Name == PartyMember.PRIEST_NAME);
      rogue = game.ActiveParty.PartyMembers.Find(x => x.Name == PartyMember.ROGUE_NAME);
    }

    [SetUp]
    public void ResetStatus()
    {
      foreach (PartyMember partyMember in game.ActiveParty.PartyMembers)
      {
        partyMember.Current = partyMember.Max;
      }

      game.ActiveEnemy.ResetStatus();
      game.StartCombatGame();
    }
    
    [Test]
    public void TestMeleeMiss()
    {
      EnemyAbility melee = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilityMelee.NAME);
      melee.MissChance = 1.0;

      game.EnemyCastHandler.TickAbility(melee, AbilityMelee.COOLDOWN);
      Assert.AreEqual(tank.Max, tank.Current, "HP");
      Assert.AreEqual(melee.Cooldown, melee.CooldownRemaining, "Cooldown");
    }

    [Test]
    public void TestMeleeHit()
    {
      EnemyAbility ability = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilityMelee.NAME);
      ability.MissChance = 0.0;

      game.EnemyCastHandler.TickAbility(ability, AbilityMelee.COOLDOWN);
      Assert.AreEqual(ability.Cooldown, ability.CooldownRemaining);

      int damageTaken = (int)(tank.Max - tank.Current);
      Assert.GreaterOrEqual(ability.BASE_MAX, damageTaken, "Max");
      Assert.LessOrEqual(ability.BASE_MIN, damageTaken, "Min");
    }

    private const double TEST_DAMAGE_MULTIPLIER = 2.0;
    [Test]
    public void TestEnemyDamageMultiplier()
    {
      game.ActiveEnemy.DamageModifier = TEST_DAMAGE_MULTIPLIER;
      EnemyAbility ability = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilityMelee.NAME);
      ability.MissChance = 0.0;

      game.EnemyCastHandler.TickAbility(ability, AbilityMelee.COOLDOWN);
      Assert.AreEqual(ability.CooldownRemaining, ability.Cooldown, "Cooldown");

      int damageTaken = (int)(tank.Max - tank.Current);
      Assert.GreaterOrEqual(ability.BASE_MAX * TEST_DAMAGE_MULTIPLIER, damageTaken, "Max");
      Assert.LessOrEqual(ability.BASE_MIN * TEST_DAMAGE_MULTIPLIER, damageTaken, "Min");

      game.ActiveEnemy.DamageModifier = 1.0;
    }

    [Test]
    public void TestMeleeWithFrenzy()
    {
      game.ActiveEnemy.AddBuff(new BuffFrenzy());
      EnemyAbility ability = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilityMelee.NAME);
      ability.MissChance = 0.0;

      game.EnemyCastHandler.TickAbility(ability, AbilityMelee.COOLDOWN);
      Assert.AreEqual(ability.CooldownRemaining, ability.Cooldown * BuffFrenzy.COOLDOWN_MODIFIER, "Cooldown");

      int damageTaken = (int)(tank.Max - tank.Current);
      Assert.GreaterOrEqual(ability.BASE_MAX * BuffFrenzy.DAMAGE_MULTIPLIER, damageTaken, "Max");
      Assert.LessOrEqual(ability.BASE_MIN * BuffFrenzy.DAMAGE_MULTIPLIER, damageTaken, "Min");
    }

    [Test]
    public void TestFrenzy()
    {
      EnemyAbility ability = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilityFrenzy.NAME);

      game.EnemyCastHandler.TickAbility(ability, AbilityFrenzy.COOLDOWN + EnemyAbility.USE_DELAY_RANGE);
      Buff buff = game.ActiveEnemy.FindBuff(AbilityFrenzy.NAME);
      Assert.AreEqual(BuffFrenzy.DURATION, buff.DurationRemaining, "Duration");
    }

    [Test]
    public void TestDelayRange()
    {
      EnemyAbility ability = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilityFrenzy.NAME);

      game.EnemyCastHandler.TickAbility(ability, AbilityFrenzy.COOLDOWN + EnemyAbility.USE_DELAY_RANGE);
      Assert.GreaterOrEqual(AbilityFrenzy.COOLDOWN + EnemyAbility.USE_DELAY_RANGE, ability.CooldownRemaining, "Max");
      Assert.LessOrEqual(AbilityFrenzy.COOLDOWN, ability.CooldownRemaining, "Min");
    }

    [Test]
    public void TestImpaleHit()
    {
      EnemyAbility ability = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilityImpale.NAME);

      List<PartyMember> target = new List<PartyMember>() { tank };
      ability.ExecuteCast(target);

      int damageTaken = (int)(tank.Max - tank.Current);
      Assert.GreaterOrEqual(ability.BASE_MAX, damageTaken, "Max");
      Assert.LessOrEqual(ability.BASE_MIN, damageTaken, "Min");
    }

    [Test]
    public void TestImpaleTick()
    {
      BuffImpale buff = BuffImpale.ApplyToTarget(tank, game);
      buff.Timer = BuffImpale.TICK_INTERVAL;
      game.ActiveEnemy.Tick(0.0);

      int damageTaken = (int)(tank.Max - tank.Current);
      Assert.GreaterOrEqual(BuffImpale.BASE_MAX, damageTaken, "Max");
      Assert.LessOrEqual(BuffImpale.BASE_MIN, damageTaken, "Min");
    }

    [Test]
    public void TestSmash()
    {
      EnemyAbility ability = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilitySmash.NAME);

      ability.ExecuteCast(game.ActiveParty.PartyMembers);

      int damageTaken = (int)(tank.Max - tank.Current);
      Assert.GreaterOrEqual(ability.BASE_MAX, damageTaken, "Max");
      Assert.LessOrEqual(ability.BASE_MIN, damageTaken, "Min");
    }

    [Test]
    public void TestSWD()
    {
      EnemyAbility ability = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilitySWD.NAME);

      ability.ExecuteCast(game.ActiveParty.PartyMembers);

      int damageTaken = (int)(shadowPriest.Max - shadowPriest.Current);
      Assert.GreaterOrEqual(ability.BASE_MAX, damageTaken, "Max");
      Assert.LessOrEqual(ability.BASE_MIN, damageTaken, "Min");
    }

    [Test]
    public void TestSWDThreshold()
    {
      EnemyAbility ability = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilitySWD.NAME);

      shadowPriest.Current = (int)(AbilitySWD.HEALTH_THRESHOLD * shadowPriest.Max);
      shadowPriest.Current--;
      double initialHealth = shadowPriest.Current;

      ability.ExecuteCast(game.ActiveParty.PartyMembers);
      Assert.AreEqual(initialHealth, shadowPriest.Current);
    }

    [Test]
    public void TestVolley()
    {
      EnemyAbility ability = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilityVolley.NAME);

      List<PartyMember> targets = game.ActiveParty.PartyMembers;
      ability.ExecuteCast(targets);

      foreach (PartyMember target in targets)
      {
        int damageTaken = (int)(target.Max - target.Current);
        Assert.GreaterOrEqual(ability.BASE_MAX, damageTaken, "Max on " + target.Name);
        Assert.LessOrEqual(ability.BASE_MIN, damageTaken, "Min on " + target.Name);
      }
    }

    [Test]
    public void TestWhirlwind()
    {
      EnemyAbility ability = game.ActiveEnemy.Abilities.Find(x => x.Name == AbilityWhirlwind.NAME);

      ability.ExecuteCast(game.ActiveParty.PartyMembers);

      int damageTaken = (int)(tank.Max - tank.Current);
      Assert.GreaterOrEqual(ability.BASE_MAX, damageTaken, "Max normal");
      Assert.LessOrEqual(ability.BASE_MIN, damageTaken, "Min normal");

      int rogueDamageTaken = (int)(rogue.Max - rogue.Current);
      Assert.GreaterOrEqual(ability.BASE_MAX * AbilityWhirlwind.ROGUE_MULTIPLIER, rogueDamageTaken, "Max rogue");
      Assert.LessOrEqual(ability.BASE_MIN * AbilityWhirlwind.ROGUE_MULTIPLIER, rogueDamageTaken, "Min rogue");
    }

    [Test]
    public void TestVampiricEmbrace()
    {
      shadowPriest.Current = 1;
      BuffVampiricEmbrace buff = (BuffVampiricEmbrace)game.ActiveEnemy.FindBuff(BuffVampiricEmbrace.NAME);
      buff.Timer = BuffVampiricEmbrace.TICK_INTERVAL;
      game.ActiveEnemy.Tick(0.0);

      int amountHealed = (int)(shadowPriest.Current - 1);
      Assert.GreaterOrEqual(BuffVampiricEmbrace.BASE_MAX, amountHealed, "Max");
      Assert.LessOrEqual(BuffVampiricEmbrace.BASE_MIN, amountHealed, "Min");
    }

  }
}
