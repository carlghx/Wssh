using System;
using System.Collections.Generic;

using NUnit.Framework;
using Wssh;
using Wssh.Entities;
using Wssh.Abilities;

namespace WsshTest
{
  [TestFixture]
  [Description("Tests relating to game settings")]
  class TestProfile
  {
    GameState game;
    Profile profile;

    [TestFixtureSetUp]
    public void TestMakeSettings()
    {
      game = new GameState();
      profile = Profile.LoadProfileFromGame(game);
    }

    [Test]
    public void TestDefaultKeys()
    {
      GameState baseGame = new GameState();
      Profile baseProfile = Profile.LoadProfileFromGame(baseGame);

      Assert.AreEqual(AbilityAvengingWrath.DEFAULT_KEY, baseProfile.KeyAW, "AW");
      Assert.AreEqual(AbilityBeaconOfLight.DEFAULT_KEY, baseProfile.KeyBOL, "BOL");
      Assert.AreEqual(MainWindow.DEFAULT_CANCEL_KEY, baseProfile.KeyCancel, "Cancel");
      Assert.AreEqual(AbilityDivineLight.DEFAULT_KEY, baseProfile.KeyDL, "DL");
      Assert.AreEqual(AbilityDivinePlea.DEFAULT_KEY, baseProfile.KeyDP, "DP");
      Assert.AreEqual(AbilityFlashOfLight.DEFAULT_KEY, baseProfile.KeyFOL, "FOL");
      Assert.AreEqual(AbilityHolyLight.DEFAULT_KEY, baseProfile.KeyHL, "HL");
      Assert.AreEqual(AbilityHolyShock.DEFAULT_KEY, baseProfile.KeyHS, "HS");
      Assert.AreEqual(AbilityJudgment.DEFAULT_KEY, baseProfile.KeyJudge, "Judge");
      Assert.AreEqual(AbilityLightOfDawn.DEFAULT_KEY, baseProfile.KeyLOD, "LOD");
      Assert.AreEqual(AbilityWordOfGlory.DEFAULT_KEY, baseProfile.KeyWOG, "WOG");
    }

    [Test]
    public void TestPlayerSettings()
    {
      Assert.AreEqual((int)(game.ActivePlayer.CritChance * 100), profile.CritPercent, "Crit");
      Assert.AreEqual((int)(game.ActivePlayer.Haste * 100), profile.HastePercent, "Haste");
      Assert.AreEqual(game.ActivePlayer.Intellect, profile.Intellect, "Int");
      Assert.AreEqual(game.ActivePlayer.Mastery, profile.Mastery, "Mastery");
      Assert.AreEqual(game.ActivePlayer.Spirit, profile.Spirit, "Spirit");
    }

    [Test]
    public void TestEnemySettings()
    {
      Assert.AreEqual((int)(game.ActiveEnemy.DamageModifier * 100), profile.EnemyDamagePercent);
    }

    private const string MODIFY_KEY = "L";
    private const int MODIFY_STAT = 120;
    private const int MODIFY_PERCENT = 50;
    private const int MODIFY_MASTERY = 10;
    [Test]
    public void TestModifyProfile()
    {
      GameState modifyGame = new GameState();
      Profile modifyProfile = MakeModifiedProfile();
      modifyGame.UpdateFromProfile(modifyProfile);

      Dictionary<Type, PlayerAbility> abilities = modifyGame.ActivePlayer.Abilities;
      Assert.AreEqual(MODIFY_KEY, abilities[typeof(AbilityAvengingWrath)].Hotkey.KeyString, "AW");
      Assert.AreEqual(MODIFY_KEY, abilities[typeof(AbilityBeaconOfLight)].Hotkey.KeyString, "BOL");
      Assert.AreEqual(MODIFY_KEY, abilities[typeof(AbilityDivineLight)].Hotkey.KeyString, "DL");
      Assert.AreEqual(MODIFY_KEY, abilities[typeof(AbilityDivinePlea)].Hotkey.KeyString, "DP");
      Assert.AreEqual(MODIFY_KEY, abilities[typeof(AbilityFlashOfLight)].Hotkey.KeyString, "FOL");
      Assert.AreEqual(MODIFY_KEY, abilities[typeof(AbilityHolyLight)].Hotkey.KeyString, "HL");
      Assert.AreEqual(MODIFY_KEY, abilities[typeof(AbilityHolyShock)].Hotkey.KeyString, "HS");
      Assert.AreEqual(MODIFY_KEY, abilities[typeof(AbilityJudgment)].Hotkey.KeyString, "Judge");
      Assert.AreEqual(MODIFY_KEY, abilities[typeof(AbilityLightOfDawn)].Hotkey.KeyString, "LOD");
      Assert.AreEqual(MODIFY_KEY, abilities[typeof(AbilityWordOfGlory)].Hotkey.KeyString, "WOG");
      Assert.AreEqual(MODIFY_KEY, modifyGame.CancelKey.KeyString, "Cancel");

      Assert.AreEqual(MODIFY_STAT,modifyGame.ActivePlayer.Intellect, "Int");
      Assert.AreEqual((double)MODIFY_PERCENT / 100, modifyGame.ActivePlayer.Haste, "Haste");
      Assert.AreEqual((double)MODIFY_PERCENT / 100, modifyGame.ActivePlayer.CritChance, "Crit");
      Assert.AreEqual(MODIFY_MASTERY, modifyGame.ActivePlayer.Mastery, "Mastery");
      Assert.AreEqual(MODIFY_STAT, modifyGame.ActivePlayer.Spirit, "Spirit");
    }

    private Profile MakeModifiedProfile()
    {
      GameState modifyGame = new GameState();
      Profile profile = Profile.LoadProfileFromGame(modifyGame);
      profile.KeyAW = MODIFY_KEY;
      profile.KeyBOL = MODIFY_KEY;
      profile.KeyCancel = MODIFY_KEY;
      profile.KeyDL = MODIFY_KEY;
      profile.KeyDP = MODIFY_KEY;
      profile.KeyFOL = MODIFY_KEY;
      profile.KeyHL = MODIFY_KEY;
      profile.KeyHS = MODIFY_KEY;
      profile.KeyJudge = MODIFY_KEY;
      profile.KeyLOD = MODIFY_KEY;
      profile.KeyWOG = MODIFY_KEY;
      profile.Intellect = MODIFY_STAT;
      profile.HastePercent = MODIFY_PERCENT;
      profile.CritPercent = MODIFY_PERCENT;
      profile.Mastery = MODIFY_MASTERY;
      profile.Spirit = MODIFY_STAT;

      return profile;
    }

  }
}
