using System;

using NUnit.Framework;
using Wssh.Entities;
using Wssh.Buffs;
using Wssh.Utilities;
using Wssh.Abilities;

namespace WsshTest
{
  [TestFixture]
  [Description("Tests relating to button and key presses")]
  public class TestUserInput
  {
    GameState game;

    [TestFixtureSetUp]
    public void TestMakeGame()
    {
      game = new GameState();
      game.StopUpdateTimer();
    }

    [Test]
    public void TestStartCombat()
    {
      game.StartCombatGame();

      Assert.AreEqual(game.ConsoleUI.InCombat, true, "InCombat");
      Assert.AreEqual(game.ConsoleUI.ElapsedTime, 0, "Elapsed time");
      Assert.LessOrEqual(DateTime.Now.Ticks - game.LastTick, 500000, "LastTick update");
      Assert.IsNotNull(game.EnemyCastHandler, "EnemyCastHandler");
      Assert.IsNotNull(game.ActiveEnemy.FindBuff(BuffVampiricEmbrace.NAME), "BuffVampiricEmbrace");
    }

    [Test]
    public void TestStopCombat()
    {
      game.StopCombatGame();

      Assert.IsFalse(game.ConsoleUI.InCombat);
    }

    [Test]
    public void TestKeyLeftCtrl()
    {
      game.HandleKeyStringDown(KeyModHandler.LEFT_CTRL);
      Assert.IsTrue(game.KeyHandler.CtrlDown, "Key down");
      game.HandleKeyStringUp(KeyModHandler.LEFT_CTRL);
      Assert.IsFalse(game.KeyHandler.CtrlDown, "Key up");
    }

    [Test]
    public void TestKeyRightCtrl()
    {
      game.HandleKeyStringDown(KeyModHandler.RIGHT_CTRL);
      Assert.IsTrue(game.KeyHandler.CtrlDown, "Key down");
      game.HandleKeyStringUp(KeyModHandler.RIGHT_CTRL);
      Assert.IsFalse(game.KeyHandler.CtrlDown, "Key up");
    }

    [Test]
    public void TestKeyLeftShift()
    {
      game.HandleKeyStringDown(KeyModHandler.LEFT_SHIFT);
      Assert.IsTrue(game.KeyHandler.ShiftDown, "Key down");
      game.HandleKeyStringUp(KeyModHandler.LEFT_SHIFT);
      Assert.IsFalse(game.KeyHandler.ShiftDown, "Key up");
    }

    [Test]
    public void TestKeyRightShift()
    {
      game.HandleKeyStringDown(KeyModHandler.RIGHT_SHIFT);
      Assert.IsTrue(game.KeyHandler.ShiftDown, "Key down");
      game.HandleKeyStringUp(KeyModHandler.RIGHT_SHIFT);
      Assert.IsFalse(game.KeyHandler.ShiftDown, "Key up");
    }

    [Test]
    public void TestKeyAvengingWrath()
    {
      TestKeyHotkey(typeof(AbilityAvengingWrath));
    }

    [Test]
    public void TestKeyBeaconOfLight()
    {
      TestKeyHotkey(typeof(AbilityBeaconOfLight));
    }

    [Test]
    public void TestKeyDivinePlea()
    {
      TestKeyHotkey(typeof(AbilityDivinePlea));
    }

    [Test]
    public void TestKeyFlashOfLight()
    {
      TestKeyHotkey(typeof(AbilityFlashOfLight));
    }

    [Test]
    public void TestKeyHolyLight()
    {
      TestKeyHotkey(typeof(AbilityHolyLight));
    }

    [Test]
    public void TestKeyJudgment()
    {
      TestKeyHotkey(typeof(AbilityJudgment));
    }

    [Test]
    public void TestKeyLightOfDawn()
    {
      game.ActivePlayer.HolyPower = 1;
      TestKeyHotkey(typeof(AbilityLightOfDawn));
    }

    [Test]
    public void TestKeyWordOfGlory()
    {
      game.ActivePlayer.HolyPower = 1;
      TestKeyHotkey(typeof(AbilityWordOfGlory));
    }

    [Test]
    public void TestCancel()
    {
      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityHolyLight)];
      game.PlayerCastHandler.UpdateCastAdd(ability);

      game.HandleKeyStringDown(game.CancelKey.KeyString);
      Assert.IsNull(game.PlayerCastHandler.CastCurrent);
    }

    [Test]
    public void TestQueueing()
    {
      ClearCooldowns();

      PlayerAbility ability = game.ActivePlayer.Abilities[typeof(AbilityHolyLight)];
      game.PlayerCastHandler.UpdateCastAdd(ability);
      game.PlayerCastHandler.UpdateCastAdd(ability);
      Assert.AreEqual(game.PlayerCastHandler.CastNext.Ability, ability, "Queue ability");

      game.HandleKeyStringDown(game.CancelKey.KeyString);
      Assert.IsNull(game.PlayerCastHandler.CastCurrent, "Clear queue");
    }

    private void TestKeyHotkey(Type abilityType)
    {
      ClearCooldowns();
      PlayerAbility ability = game.ActivePlayer.Abilities[abilityType];
      game.HandleKeyStringDown(ability.Hotkey.KeyString);
      Assert.AreEqual(game.PlayerCastHandler.CastCurrent.Ability, ability, "Use key");
      
      game.HandleKeyStringDown(game.CancelKey.KeyString);
      Assert.IsNull(game.PlayerCastHandler.CastCurrent, "Cancel");
    }

    private void ClearCooldowns()
    {
      foreach (PlayerAbility ability in game.ActivePlayer.Abilities.Values)
      {
        ability.CooldownRemaining = 0;
      }
    }

  }
}
