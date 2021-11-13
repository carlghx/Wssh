using System;
using NUnit.Framework;

using Wssh.Entities;
using Wssh.UI;
using Wssh.Abilities;
using Wssh.Abilities.EnemyAbilities;


namespace WsshTest
{
  [TestFixture]
  [Description("Tests relating to game initialization")]
  public class TestGameInit
  {
    GameState game;

    [TestFixtureSetUp]
    public void TestMakeGame()
    {
      game = new GameState();
    }

    [Test]
    public void TestInitOOMUI()
    {
      Assert.IsNotNull(game.OOMUI);
    }

    [Test]
    public void TestPlayerManaUI()
    {
      Assert.IsNotNull(game.ActivePlayer.PlayerManaUI);
    }

    [Test]
    public void TestPlayerHolyUI()
    {
      Assert.IsNotNull(game.ActivePlayer.HolyPowerUI);
    }

    [Test]
    public void TestLastCastUI()
    {
      Assert.IsNotNull(game.ActivePlayer.LastCast);
    }

    [Test]
    public void TestPlayerFrameUI()
    {
      Assert.IsNotNull(game.ActivePlayer.PartyFrameUI);
    }

    [Test]
    public void TestPlayerManaMax()
    {
      Assert.AreEqual(game.ActivePlayer.ManaMax, game.ActivePlayer.PlayerManaUI.Max, "ManaMax");      
    }

    [Test]
    public void TestPlayerManaCurrent()
    {
      Assert.AreEqual(game.ActivePlayer.ManaCurrent, game.ActivePlayer.PlayerManaUI.Max, "ManaCurrent");
    }

    [Test]
    public void TestSpellbookHolyLight()
    {
      AbilityHolyLight ability = game.ActivePlayer.Abilities[typeof(AbilityHolyLight)] as AbilityHolyLight;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestSpellbookBeaconOfLight()
    {
      AbilityBeaconOfLight ability = game.ActivePlayer.Abilities[typeof(AbilityBeaconOfLight)] as AbilityBeaconOfLight;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestSpellbookHolyShock()
    {
      AbilityHolyShock ability = game.ActivePlayer.Abilities[typeof(AbilityHolyShock)] as AbilityHolyShock;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestSpellbookWordOfGlory()
    {
      AbilityWordOfGlory ability = game.ActivePlayer.Abilities[typeof(AbilityWordOfGlory)] as AbilityWordOfGlory;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestSpellbookFlashOfLight()
    {
      AbilityFlashOfLight ability = game.ActivePlayer.Abilities[typeof(AbilityFlashOfLight)] as AbilityFlashOfLight;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestSpellbookDivineLight()
    {
      AbilityDivineLight ability = game.ActivePlayer.Abilities[typeof(AbilityDivineLight)] as AbilityDivineLight;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestSpellbookJudgment()
    {
      AbilityJudgment ability = game.ActivePlayer.Abilities[typeof(AbilityJudgment)] as AbilityJudgment;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestSpellbookLightOfDawn()
    {
      AbilityLightOfDawn ability = game.ActivePlayer.Abilities[typeof(AbilityLightOfDawn)] as AbilityLightOfDawn;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestSpellbookDivinePlea()
    {
      AbilityDivinePlea ability = game.ActivePlayer.Abilities[typeof(AbilityDivinePlea)] as AbilityDivinePlea;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestSpellbookAvengingWrath()
    {
      AbilityAvengingWrath ability = game.ActivePlayer.Abilities[typeof(AbilityAvengingWrath)] as AbilityAvengingWrath;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestEnemy()
    {
      Assert.AreEqual(Enemy.DEFAULT_NAME, game.ActiveEnemy.Name);
    }

    [Test]
    public void TestEnemySpellbookMelee()
    {
      AbilityMelee ability = game.ActiveEnemy.Abilities.Find(new Predicate<EnemyAbility>(x => x.Name == AbilityMelee.NAME)) as AbilityMelee;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestEnemySpellbookSmash()
    {
      AbilitySmash ability = game.ActiveEnemy.Abilities.Find(new Predicate<EnemyAbility>(x => x.Name == AbilitySmash.NAME)) as AbilitySmash;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestEnemySpellbookWhirlwind()
    {
      AbilityWhirlwind ability = game.ActiveEnemy.Abilities.Find(new Predicate<EnemyAbility>(x => x.Name == AbilityWhirlwind.NAME)) as AbilityWhirlwind;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestEnemySpellbookImpale()
    {
      AbilityImpale ability = game.ActiveEnemy.Abilities.Find(new Predicate<EnemyAbility>(x => x.Name == AbilityImpale.NAME)) as AbilityImpale;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestEnemySpellbookVolley()
    {
      AbilityVolley ability = game.ActiveEnemy.Abilities.Find(new Predicate<EnemyAbility>(x => x.Name == AbilityVolley.NAME)) as AbilityVolley;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestEnemySpellbookSWD()
    {
      AbilitySWD ability = game.ActiveEnemy.Abilities.Find(new Predicate<EnemyAbility>(x => x.Name == AbilitySWD.NAME)) as AbilitySWD;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestEnemySpellbookFrenzy()
    {
      AbilityFrenzy ability = game.ActiveEnemy.Abilities.Find(new Predicate<EnemyAbility>(x => x.Name == AbilityFrenzy.NAME)) as AbilityFrenzy;
      Assert.IsNotNull(ability);
    }

    [Test]
    public void TestPartyWarrior()
    {
      PartyMember p = game.ActiveParty.PartyMembers.Find(new Predicate<PartyMember>(x => x.Name == PartyMember.WARRIOR_NAME));
      Assert.IsNotNull(p);
    }

    [Test]
    public void TestPartyRogue()
    {
      PartyMember p = game.ActiveParty.PartyMembers.Find(new Predicate<PartyMember>(x => x.Name == PartyMember.ROGUE_NAME));
      Assert.IsNotNull(p);
    }

    [Test]
    public void TestPartyPriest()
    {
      PartyMember p = game.ActiveParty.PartyMembers.Find(new Predicate<PartyMember>(x => x.Name == PartyMember.PRIEST_NAME));
      Assert.IsNotNull(p);
    }

    [Test]
    public void TestPartyMage()
    {
      PartyMember p = game.ActiveParty.PartyMembers.Find(new Predicate<PartyMember>(x => x.Name == PartyMember.MAGE_NAME));
      Assert.IsNotNull(p);
    }

    [Test]
    public void TestPartyPaladin()
    {
      PartyMember p = game.ActiveParty.PartyMembers.Find(new Predicate<PartyMember>(x => x.Name == Player.DEFAULT_NAME));
      Assert.IsNotNull(p);
    }

    [Test]
    public void TestConsole()
    {
      ConsoleUI console = game.ConsoleUI;
      Assert.IsFalse(console.InCombat);
    }

    [Test]
    public void TestPlayerCastUI()
    {
      Assert.IsNotNull(game.PlayerCastUI);
    }

    [Test]
    public void TestLastTick()
    {
      Assert.Less(0, game.LastTick);
    }

    [Test]
    public void TestPlayerCastHandler()
    {
      Assert.AreEqual(game.ActiveParty.PartyMembers[0], game.PlayerCastHandler.CurrentTarget);
    }

  }
}
