using System;
using System.Collections.Generic;

using NUnit.Framework;
using Wssh.Entities;
using Wssh.Abilities;
using Wssh.Buffs;
using Wssh.Abilities.EnemyAbilities;

namespace WsshTest
{
  [TestFixture]
  [Description("Tests relating to player buffs and abilities")]
  public class TestPlayerCast
  {
    GameState game;

    [TestFixtureSetUp]
    public void TestMakeGame()
    {
      game = new GameState();
      game.StopUpdateTimer();
    }

    private Cast MakeCast(Type abilityType, PartyMember target)
    {
      return MakeCast(abilityType, new List<PartyMember>() { target });
    }

    private Cast MakeCast(Type abilityType, List<PartyMember> targets)
    {
      Ability ability = game.ActivePlayer.Abilities[abilityType];
      Cast cast = new Cast { Ability = ability, Targets = targets };
      
      return cast;
    }

    [SetUp]
    public void ResetStatus()
    {
      game.ActivePlayer.ResetStatus();
      game.ActivePlayer.CritChance = 0;
    }


    [Test]
    public void TestProtectorOfTheInnocent()
    {
      PartyMember target = game.ActiveParty.PartyMembers.Find(x => x.Name == PartyMember.WARRIOR_NAME);
      game.ActivePlayer.PartyFrameUI.Current = 1;

      Cast cast = MakeCast(typeof(AbilityHolyLight), target);
      cast.ExecuteCast();

      double amountHealed = game.ActivePlayer.PartyFrameUI.Current - 1;
      int expectedHealBase = (int)(game.ActivePlayer.SpellPower * Player.PROTECTOR_OF_INNOCENT_MULTIPLIER);
      Assert.GreaterOrEqual(expectedHealBase + Player.PROTECTOR_OF_INNOCENT_MAX, amountHealed, "Max");
      Assert.LessOrEqual(expectedHealBase + Player.PROTECTOR_OF_INNOCENT_MIN, amountHealed, "Min");
    }

    [Test]
    public void TestBeaconOfLightHeal()
    {
      PartyMember beaconTarget = game.ActiveParty.PartyMembers.Find(x => x.Name == PartyMember.WARRIOR_NAME);
      BuffBeaconOfLight beacon = new BuffBeaconOfLight(beaconTarget);
      game.ActivePlayer.AddUniqueBuff(beacon, game);

      beaconTarget.Current = 1;

      Cast cast = MakeCast(typeof(AbilityHolyLight), game.ActivePlayer.PartyFrameUI);
      cast.ExecuteCast();

      int expectedMin, expectedMax;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityHolyLight)];
      GetExpectedHealAmounts(ability, out expectedMin, out expectedMax);
      expectedMin = (int)(expectedMin * AbilityBeaconOfLight.HEAL_MODIFIER);
      expectedMax = (int)(expectedMax * AbilityBeaconOfLight.HEAL_MODIFIER);

      double amountHealed = beaconTarget.Current - 1;
      Assert.GreaterOrEqual(expectedMax, amountHealed, "Max");
      Assert.LessOrEqual(expectedMin, amountHealed, "Min");
    }

    [Test]
    public void TestCrit()
    {
      game.ActivePlayer.CritChance = 1.0;
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityHolyLight)];

      int expectedMin, expectedMax;
      GetExpectedHealAmounts(ability, out expectedMin, out expectedMax);
      expectedMin = (int)(expectedMin * PlayerAbility.CRIT_MULTIPLIER);
      expectedMax = (int)(expectedMax * PlayerAbility.CRIT_MULTIPLIER);

      TestHeal(ability, target, expectedMin, expectedMax);
    }

    [Test]
    public void TestAvengingWrathBonus()
    {
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityHolyLight)];
      game.ActivePlayer.AddBuff(new BuffAvengingWrath(game.ActivePlayer.PartyFrameUI), game);

      int expectedMin, expectedMax;
      GetExpectedHealAmounts(ability, out expectedMin, out expectedMax, BuffAvengingWrath.MODIFIER);

      TestHeal(ability, target, expectedMin, expectedMax);
    }

    [Test]
    public void TestDivinePleaPenalty()
    {
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityHolyLight)];
      game.ActivePlayer.AddBuff(new BuffDivinePlea(game.ActivePlayer.PartyFrameUI), game);

      int expectedMin, expectedMax;
      GetExpectedHealAmounts(ability, out expectedMin, out expectedMax, BuffDivinePlea.HEAL_MODIFIER);

      TestHeal(ability, target, expectedMin, expectedMax);
    }

    [Test]
    public void TestConvictionBonus()
    {
      game.ActivePlayer.CritChance = 0.0;
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityHolyLight)];
      for(int i = 0; i < BuffConviction.MAX_STACKS; i++)
        game.ActivePlayer.AddConvictionBuff(game);

      int expectedMin, expectedMax;
      GetExpectedHealAmounts(ability, out expectedMin, out expectedMax, game.ActivePlayer.ConvictionMultiplier());

      TestHeal(ability, target, expectedMin, expectedMax);
    }

    [Test]
    public void TestHolyLight()
    {
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityHolyLight)];

      int expectedMin, expectedMax;
      GetExpectedHealAmounts(ability, out expectedMin, out expectedMax);
      TestHeal(ability, target, expectedMin, expectedMax);
    }

    /// <summary>
    /// Get the base min and base max heal amounts for a given ability
    /// </summary>
    /// <param name="ability">Ability to get min and max amounts for</param>
    /// <param name="expectedMin">Expected heal amount using the minimum random number for that heal</param>
    /// <param name="expectedMax">Expected heal amount using the maximum random number for that heal</param>
    /// <param name="crit">Whether this ability is going to crit</param>
    private void GetExpectedHealAmounts(PlayerAbility ability, out int expectedMin, out int expectedMax, double extraMultiplier = 1.0)
    {
      double dExpectedMin = (ability.BASE_MIN + ability.POWER_MULTIPLIER * game.ActivePlayer.SpellPower) * Player.HEALING_MULTIPLIER;
      double dExpectedMax = (ability.BASE_MAX + ability.POWER_MULTIPLIER * game.ActivePlayer.SpellPower) * Player.HEALING_MULTIPLIER;

      dExpectedMin *= extraMultiplier;
      dExpectedMax *= extraMultiplier;

      expectedMin = (int)(dExpectedMin);
      expectedMax = (int)(dExpectedMax);
    }


    [Test]
    public void TestFlashOfLight()
    {
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityFlashOfLight)];

      int expectedMin, expectedMax;
      GetExpectedHealAmounts(ability, out expectedMin, out expectedMax);
      TestHeal(ability, target, expectedMin, expectedMax);
    }

    [Test]
    public void TestDivineLight()
    {
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityDivineLight)];

      int expectedMin, expectedMax;
      GetExpectedHealAmounts(ability, out expectedMin, out expectedMax);
      TestHeal(ability, target, expectedMin, expectedMax);
    }

    [Test]
    public void TestHolyShock()
    {
      game.ActivePlayer.CritChance = -AbilityHolyShock.CRIT_CHANCE_BONUS;
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityHolyShock)];
      
      int expectedMin, expectedMax;
      GetExpectedHealAmounts(ability, out expectedMin, out expectedMax);
      TestHeal(ability, target, expectedMin, expectedMax);
      Assert.AreEqual(DEFAULT_HOLY_POWER + 1, game.ActivePlayer.HolyPower, "Holy power");
    }

    [Test]
    public void TestLightOfDawn()
    {
      List<PartyMember> targets = new List<PartyMember>();
      foreach (PartyMember partyMember in game.ActiveParty.PartyMembers)
        targets.Add(partyMember);

      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityLightOfDawn)];

      int expectedMin, expectedMax;
      GetExpectedHealAmounts(ability, out expectedMin, out expectedMax);
      TestHeal(ability, targets, expectedMin, expectedMax);

      TestHeal(ability, targets, expectedMin, expectedMax, 3);
    }

    [Test]
    public void TestWordOfGlory()
    {
      game.ActivePlayer.CritChance = -AbilityWordOfGlory.LAST_WORD_CRIT_CHANCE_BONUS;
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityWordOfGlory)];

      int expectedMin, expectedMax;
      GetExpectedHealAmounts(ability, out expectedMin, out expectedMax);
      TestHeal(ability, target, expectedMin, expectedMax);

      TestHeal(ability, target, expectedMin, expectedMax, 3);
    }

    [Test]
    public void TestLastWord()
    {
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      target.Current = 1;
      game.ActivePlayer.CritChance = 1 - AbilityWordOfGlory.LAST_WORD_CRIT_CHANCE_BONUS;
      game.ActivePlayer.HolyPower = 1;
      List<PartyMember> targets = new List<PartyMember>() { target };
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityWordOfGlory)];
      
      Cast cast = new Cast() { Ability = ability, Targets = targets };
      game.PlayerCastHandler.ForceCast(cast);

      Buff convictionBuff = game.ActivePlayer.FindBuff(BuffConviction.NAME);
      Assert.AreEqual(1, convictionBuff.Amount, "Conviction");
    }

    private const int DEFAULT_HOLY_POWER = 1;
    private void TestHeal(PlayerAbility ability, PartyMember target, int expectedMin, int expectedMax, int holyPower = DEFAULT_HOLY_POWER)
    {
      TestHeal(ability, new List<PartyMember>() { target }, expectedMin, expectedMax);
    }

    private void TestHeal(PlayerAbility ability, List<PartyMember> targets, int expectedMin, int expectedMax, int holyPower = DEFAULT_HOLY_POWER)
    {
      SetPreHealConditions(targets);
      game.ActivePlayer.HolyPower = holyPower;

      expectedMin *= holyPower;
      expectedMax *= holyPower;
      Cast cast = new Cast() { Ability = ability, Targets = targets };
      game.PlayerCastHandler.ForceCast(cast);

      foreach (PartyMember target in targets)
      {
        string message = " on " + target.Name;
        if (ability.UsesHolyPower)
          message = message + " with holy power " + holyPower;
        double amountHealed = target.Current - 1;
        Assert.GreaterOrEqual(expectedMax, amountHealed, "Max " + message);
        Assert.LessOrEqual(expectedMin, amountHealed, "Min " + message);
      }

      Assert.AreEqual(game.ActivePlayer.ManaMax - ability.ManaCost, game.ActivePlayer.ManaCurrent, "Mana cost");

      if (ability.UsesHolyPower)
        Assert.AreEqual(0, game.ActivePlayer.HolyPower, "Holy power cost");
    }

    private void SetPreHealConditions(List<PartyMember> targets)
    {
      foreach (PartyMember target in targets)
        target.Current = 1;

      game.ActivePlayer.ManaCurrent = game.ActivePlayer.ManaMax;
    }

    [Test]
    public void TestBeaconOfLight()
    {
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityBeaconOfLight)];
      TestBuffCast(ability, target);
    }

    [Test]
    public void TestAvengingWrath()
    {
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityAvengingWrath)];
      TestBuffCast(ability, target);
    }

    [Test]
    public void TestDivinePlea()
    {
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityDivinePlea)];
      TestBuffCast(ability, target);
    }

    [Test]
    public void TestJudgment()
    {
      double oldHaste = game.ActivePlayer.Haste;
      game.ActivePlayer.ManaCurrent = AbilityJudgment.COST;

      Cast cast = MakeCast(typeof(AbilityJudgment), game.ActivePlayer.PartyFrameUI);
      game.PlayerCastHandler.ForceCast(cast);

      Assert.AreEqual(AbilityJudgment.COST * 3, game.ActivePlayer.ManaCurrent, "Mana gain");

      PlayerAbility judgment = game.ActivePlayer.Abilities[typeof(AbilityJudgment)];
      if (judgment.Cooldown > PlayerAbility.GCD)
        Assert.AreEqual(judgment.Cooldown, judgment.CooldownRemaining, "Cooldown");

      double newHaste = game.ActivePlayer.Haste;
      Assert.AreEqual(oldHaste + BuffJudgment.HASTE, newHaste, "Haste gain");
    }

    private void TestBuffCast(PlayerAbility ability, PartyMember target)
    {
      Cast cast = new Cast() { Ability = ability, Targets = new List<PartyMember>() { target } };
      game.PlayerCastHandler.ForceCast(cast);

      Assert.AreEqual(game.ActivePlayer.ManaCurrent, game.ActivePlayer.ManaMax - ability.ManaCost, "Mana cost");
      if (ability.Cooldown > PlayerAbility.GCD)
        Assert.AreEqual(ability.CooldownRemaining, ability.Cooldown, "Cooldown");
      Assert.IsNotNull(game.ActivePlayer.FindBuffOnTarget(ability.Name, target), "Buff exists");
    }


    [Test]
    public void TestGCD()
    {
      List<PartyMember> targets = new List<PartyMember>() { game.ActivePlayer.PartyFrameUI };
      SetPreHealConditions(targets);
      PlayerAbility abilityToCast = game.ActivePlayer.Abilities[typeof(AbilityHolyLight)];

      game.PlayerCastHandler.UpdateCastAdd(abilityToCast);
      foreach (PlayerAbility ability in game.ActivePlayer.Abilities.Values)
      {
        if (ability.UsesGCD)
          Assert.AreEqual(game.ActivePlayer.GetModifiedGCD(), ability.CooldownRemaining);
      }
    }

    [Test]
    public void TestConviction()
    {
      game.ActivePlayer.CritChance = 1.0;
      Cast cast = MakeCast(typeof(AbilityHolyLight), game.ActivePlayer.PartyFrameUI);

      int castNumber;
      Buff conviction;
      // make sure it stacks up to maximum
      for (castNumber = 1; castNumber < BuffConviction.MAX_STACKS; castNumber++)
      {
        game.PlayerCastHandler.ForceCast(cast);
        conviction = game.ActivePlayer.FindBuff(BuffConviction.NAME);
        Assert.AreEqual(castNumber, conviction.Amount);
      }

      // make sure it doesn't stack past maximum
      game.PlayerCastHandler.ForceCast(cast);
      conviction = game.ActivePlayer.FindBuff(BuffConviction.NAME);
      Assert.AreEqual(castNumber, conviction.Amount, "Stack max");
    }

    [Test]
    public void TestDaybreakHolyLight()
    {
      TestDaybreakAbility(game.ActivePlayer.Abilities[typeof(AbilityHolyLight)]);
    }

    [Test]
    public void TestDaybreakFlashOfLight()
    {
      TestDaybreakAbility(game.ActivePlayer.Abilities[typeof(AbilityFlashOfLight)]);
    }

    [Test]
    public void TestDaybreakDivineLight()
    {
      TestDaybreakAbility(game.ActivePlayer.Abilities[typeof(AbilityDivineLight)]);
    }

    private void TestDaybreakAbility(PlayerAbility ability)
    {
      List<PartyMember> targets = new List<PartyMember>() { game.ActivePlayer.PartyFrameUI };
      Cast cast = new Cast() { Ability = ability, Targets = targets };

      BuffDaybreak.PROC_CHANCE = 1.0;
      game.PlayerCastHandler.ForceCast(cast);

      Assert.IsNotNull(game.ActivePlayer.FindBuff(BuffDaybreak.NAME));
    }

    [Test]
    public void TestDaybreakConsume()
    {
      BuffDaybreak daybreak = new BuffDaybreak(game.ActivePlayer.PartyFrameUI);
      game.ActivePlayer.AddBuff(daybreak, game);

      PlayerAbility holyShock = game.ActivePlayer.Abilities[typeof(AbilityHolyShock)];
      List<PartyMember> targets = new List<PartyMember>() { game.ActivePlayer.PartyFrameUI };
      Cast cast = new Cast() { Ability = holyShock, Targets = targets };
      game.PlayerCastHandler.ForceCast(cast);

      Assert.GreaterOrEqual(holyShock.Cooldown, holyShock.CooldownRemaining, "Cooldown");
      Assert.IsNull(game.ActivePlayer.FindBuff(BuffDaybreak.NAME), "Buff exists");
    }

    [Test]
    public void TestDivinePleaTick()
    {
      BuffDivinePlea divinePlea = new BuffDivinePlea(game.ActivePlayer.PartyFrameUI);
      game.ActivePlayer.AddBuff(divinePlea, game);
      divinePlea.DurationRemaining -= BuffDivinePlea.TICK_INTERVAL + 1;

      int manaRestored = (int)(game.ActivePlayer.ManaMax * BuffDivinePlea.MANA_PER_TICK);
      game.ActivePlayer.ManaCurrent -= manaRestored;
      game.ActivePlayer.Tick(0.1, game);
      Assert.AreEqual(game.ActivePlayer.ManaMax, game.ActivePlayer.ManaCurrent);
    }

    [Test]
    public void TestIlluminatedHealing()
    {
      PartyMember target = game.ActivePlayer.PartyFrameUI;
      target.Current = target.Max;
      game.ActivePlayer.CritChance = 1.0;
      Cast cast = MakeCast(typeof(AbilityHolyLight), target);

      game.PlayerCastHandler.ForceCast(cast);
      Buff buff = game.ActivePlayer.FindBuff(BuffIlluminatedHealing.NAME);
      Assert.IsNotNull(buff);

      EnemyAbility.HandleDamage(target, buff.Amount, game);
      Assert.AreEqual(target.Max, target.Current, "Prevent damage");
      Assert.IsNull(game.ActivePlayer.FindBuff(BuffIlluminatedHealing.NAME), "Buff fades");
    }

    private const double TICK_AMOUNT = 1.0;
    [Test]
    public void TestInfusion()
    {
      BuffInfusion infusion = new BuffInfusion(game.ActivePlayer.PartyFrameUI);
      game.ActivePlayer.AddBuff(infusion, game);
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityHolyLight)];
      game.PlayerCastHandler.UpdateCastAdd(ability);

      game.PlayerCastHandler.UpdateTimer_Tick(TICK_AMOUNT);
      
      double hasteBonus = game.ActivePlayer.Haste;
      double infusionHasteBonus = BuffInfusion.GetHasteBonus(AbilityHolyLight.CAST_TIME, game);

      hasteBonus += infusionHasteBonus;
      Assert.AreEqual(TICK_AMOUNT * (1 + hasteBonus), game.PlayerCastUI.Current);
    }

  }
}
