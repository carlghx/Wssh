using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

using Wssh.Utilities;
using Wssh.UI;
using Wssh.Abilities;
using Wssh.Buffs;
using System.Windows;
using Wssh.Abilities.EnemyAbilities;

namespace Wssh.Entities
{
  /// <summary>
  /// Contains and updates all current game information, independent of UI
  /// </summary>
  public class GameState
  {
    /// <summary>
    /// The player character in this game
    /// </summary>
    public Player ActivePlayer { get; set; }

    /// <summary>
    /// The enemy 'boss' in this game
    /// </summary>
    public Enemy ActiveEnemy { get; set; }

    public CombatLog Log { get; set; }

    /// <summary>
    /// The group of characters in this game
    /// </summary>
    public Party ActiveParty { get; set; }

    /// <summary>
    /// UI for displaying elapsed time and the reset button
    /// </summary>
    public ConsoleUI ConsoleUI { get; set; }

    /// <summary>
    /// UI for determining whether a given spell button should have a blue/black mask
    /// </summary>
    public OOMIndicatorUI OOMUI { get; set; }

    /// <summary>
    /// UI for player's cast bar
    /// </summary>
    public BarUI PlayerCastUI { get; set; }

    /// <summary>
    /// Current game time, in seconds
    /// </summary>
    public double CurrentSecond { get; set; }

    /// <summary>
    /// Time of last update
    /// </summary>
    public long LastTick { get; set; }

    /// <summary>
    /// Key used to cancel the current cast
    /// </summary>
    public HotkeyInfo CancelKey { get; set; }

    private static KeyModHandler _keyMod;
    /// <summary>
    /// Stores modifier keys (ctrl and shift)
    /// </summary>
    public KeyModHandler KeyHandler
    {
      get
      {
        return _keyMod;
      }
    }

    /// <summary>
    /// How often (in milliseconds) the game attempts to update.
    /// Setting this too low will slow down the game.
    /// Setting this too high will cause play to be choppy.
    /// </summary>
    public const int TIMER_INTERVAL_MS = 25;

    /// <summary>
    /// How often (in seconds) the game attempts to update
    /// </summary>
    public const double TIMER_INTERVAL_S = (double)TIMER_INTERVAL_MS / 1000;

    private DispatcherTimer UpdateTimer { get; set; }

    public PlayerCastHandler PlayerCastHandler { get; set; }
    public EnemyCastHandler EnemyCastHandler { get; set; }

    public GameState()
    {
      InitializeFirstTime();
      SetupGame();
    }

    public GameState(CombatLog log) : this()
    {
      Log = log;
    }

    /// <summary>
    /// Sends a message to current message log, if it exists
    /// </summary>
    /// <param name="message">Message to write</param>
    public void LogMessage(string message)
    {
      if (Log != null)
        Log.LogMessage(message);
      else
        Console.WriteLine(message);
    }

    /// <summary>
    /// Initialization that is only needed the first time the application is started
    /// </summary>
    private void InitializeFirstTime()
    {
      _keyMod = new KeyModHandler();
      OOMUI = new OOMIndicatorUI();

      ActivePlayer = new Player(this, OOMUI);
      ActiveEnemy = new Enemy(this);
      ActiveParty = Party.MakeDefaultParty(ActivePlayer);
      ConsoleUI = new ConsoleUI();
      PlayerCastUI = BarUI.CreateCastUI();

      CancelKey = HotkeyInfo.CreateFromKeyString(MainWindow.DEFAULT_CANCEL_KEY);

      InitUpdateTimer();
    }

    /// <summary>
    /// Initialization that has to be called after every reset or game settings change
    /// </summary>
    public void SetupGame()
    {
      LastTick = DateTime.Now.Ticks;

      ActivePlayer.ResetStatus();
      ActiveParty.ResetStatus();      

      PlayerCastHandler = new PlayerCastHandler(this);
      PlayerCastHandler.CurrentTarget = ActiveParty.PartyMembers[0];
      
      PlayerCastUI.Visible = Visibility.Collapsed;
    }

    private void InitUpdateTimer()
    {
      UpdateTimer = new DispatcherTimer();
      UpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, TIMER_INTERVAL_MS);
      UpdateTimer.Tick += new EventHandler(updateTimer_Tick);
      UpdateTimer.Start();
    }

    private void ResetTimer()
    {
      CurrentSecond = 0;
    }

    private void updateTimer_Tick(object sender, EventArgs e)
    {
      double timerIncrementSeconds = GetTimerIncrementSeconds();

      ConsoleUI.Tick(timerIncrementSeconds);
      HandlePlayerUpdates(timerIncrementSeconds);

      if (ConsoleUI.InCombat)
      {
        EnemyCastHandler.UpdateTimer_Tick(timerIncrementSeconds);
        ActiveEnemy.Tick(timerIncrementSeconds);

        CheckDeaths();
      }
    }

    private double GetTimerIncrementSeconds()
    {
      long timerIncrement = DateTime.Now.Ticks - LastTick;
      LastTick = LastTick + timerIncrement;

      return TickToSecond(timerIncrement);
    }

    private double TickToSecond(long ticks)
    {
      return ticks * 0.0000001;
    }

    private void HandlePlayerUpdates(double timerIncrementSeconds)
    {
      ActivePlayer.Tick(timerIncrementSeconds, this);

      PlayerCastHandler.UpdateTimer_Tick(timerIncrementSeconds);
    }

    /// <summary>
    /// End combat if a party member has died
    /// </summary>
    private void CheckDeaths()
    {
      foreach (PartyMember member in ActiveParty.PartyMembers)
      {
        if (!member.IsAlive())
        {
          string message = String.Format("{0} has died", member.Name);
          LogMessage(message);
          StopCombatGame();
          return;
        }
      }
    }


    /// <summary>
    /// Start combat, initializing the enemy and resetting the game clock
    /// </summary>
    public void StartCombatGame()
    {
      ConsoleUI.InCombat = true;
      ConsoleUI.ElapsedTime = 0;
      LastTick = DateTime.Now.Ticks;
      InitEnemy();
    }

    /// <summary>
    /// Halt combat and stop enemy activity
    /// </summary>
    public void StopCombatGame()
    {
      ConsoleUI.InCombat = false;
      ActiveEnemy.LastCasts.Clear();
    }

    private void InitEnemy()
    {      
      EnemyCastHandler = new EnemyCastHandler(this);      

      foreach (PartyMember member in ActiveParty.PartyMembers)
      {
        if (member.Shadow)
        {
          BuffVampiricEmbrace VE = new BuffVampiricEmbrace() { Target = member };
          ActiveEnemy.AddBuff(VE);
        }
      }
    }

    /// <summary>
    /// Adds a cooldown to all abilities on the GCD (global cooldown)
    /// </summary>
    public void TriggerGCD()
    {
      foreach (KeyValuePair<Type, PlayerAbility> pair in ActivePlayer.Abilities)
      {
        PlayerAbility ability = pair.Value;
        double currentGCD = ActivePlayer.GetModifiedGCD();
        if (ability.UsesGCD)
        {
          if (ability.CooldownRemaining < currentGCD)
            ability.CooldownRemaining = currentGCD;
        }
      }
    }

    /// <summary>
    /// Register an ability key or mouse press, adding it to the player cast handler
    /// </summary>
    /// <param name="ability"></param>
    public void UseAbility(PlayerAbility ability)
    {
      PlayerCastHandler.UpdateCastAdd(ability);
    }


    /// <summary>
    /// Check to see if any spell's hotkey has been pressed, and add that ability to the cast handler
    /// </summary>
    /// <param name="keyStr">Key string to check</param>
    public void HandleKeyStringDown(string keyStr)
    {
      KeyModHandler keyHandler = KeyHandler;
      if (keyHandler.MatchHotkey(keyStr, ActivePlayer.Abilities[typeof(AbilityWordOfGlory)].Hotkey))
        PlayerCastHandler.UpdateCastAdd(typeof(AbilityWordOfGlory));
      else if (keyHandler.MatchHotkey(keyStr, ActivePlayer.Abilities[typeof(AbilityFlashOfLight)].Hotkey))
        PlayerCastHandler.UpdateCastAdd(typeof(AbilityFlashOfLight));
      else if (keyHandler.MatchHotkey(keyStr, ActivePlayer.Abilities[typeof(AbilityDivineLight)].Hotkey))
        PlayerCastHandler.UpdateCastAdd(typeof(AbilityDivineLight));
      else if (keyHandler.MatchHotkey(keyStr, ActivePlayer.Abilities[typeof(AbilityHolyLight)].Hotkey))
        PlayerCastHandler.UpdateCastAdd(typeof(AbilityHolyLight));
      else if (keyHandler.MatchHotkey(keyStr, ActivePlayer.Abilities[typeof(AbilityBeaconOfLight)].Hotkey))
        PlayerCastHandler.UpdateCastAdd(typeof(AbilityBeaconOfLight));
      else if (keyHandler.MatchHotkey(keyStr, ActivePlayer.Abilities[typeof(AbilityHolyShock)].Hotkey))
        PlayerCastHandler.UpdateCastAdd(typeof(AbilityHolyShock));
      else if (keyHandler.MatchHotkey(keyStr, ActivePlayer.Abilities[typeof(AbilityJudgment)].Hotkey))
        PlayerCastHandler.UpdateCastAdd(typeof(AbilityJudgment));
      else if (keyHandler.MatchHotkey(keyStr, ActivePlayer.Abilities[typeof(AbilityLightOfDawn)].Hotkey))
        PlayerCastHandler.UpdateCastAdd(typeof(AbilityLightOfDawn));
      else if (keyHandler.MatchHotkey(keyStr, ActivePlayer.Abilities[typeof(AbilityDivinePlea)].Hotkey))
        PlayerCastHandler.UpdateCastAdd(typeof(AbilityDivinePlea));
      else if (keyHandler.MatchHotkey(keyStr, ActivePlayer.Abilities[typeof(AbilityAvengingWrath)].Hotkey))
        PlayerCastHandler.UpdateCastAdd(typeof(AbilityAvengingWrath));

      else if (keyStr == "Escape" || keyHandler.MatchHotkey(keyStr, CancelKey))
        PlayerCastHandler.Cancel();

      else if (keyStr == KeyModHandler.LEFT_SHIFT || keyStr == KeyModHandler.RIGHT_SHIFT)
        KeyHandler.ShiftDown = true;
      else if (keyStr == KeyModHandler.LEFT_CTRL || keyStr == KeyModHandler.RIGHT_CTRL)
        KeyHandler.CtrlDown = true;
    }


    /// <summary>
    /// Check to see if any modifier keys have been released (shift and ctrl)
    /// </summary>
    /// <param name="keyStr">Key string to check</param>
    public void HandleKeyStringUp(string keyStr)
    {
      if (keyStr == KeyModHandler.LEFT_SHIFT || keyStr == KeyModHandler.RIGHT_SHIFT)
        KeyHandler.ShiftDown = false;
      else if (keyStr == KeyModHandler.LEFT_CTRL || keyStr == KeyModHandler.RIGHT_CTRL)
        KeyHandler.CtrlDown = false;
    }

    /// <summary>
    /// Set currently targeted party member
    /// </summary>
    /// <param name="currentTarget">Party member to target</param>
    public void SetSelectedPartyMember(PartyMember currentTarget)
    {
      PlayerCastHandler.CurrentTarget = currentTarget;
    }

    /// <summary>
    /// Updates game settings by reading from a profile object
    /// </summary>
    /// <param name="profile">Profile to update with</param>
    public void UpdateFromProfile(Profile profile)
    {
      UpdatePlayerValues(profile);
      UpdateEnemyValues(profile);
      UpdateHotKeyValues(profile);
    }

    private void UpdatePlayerValues(Profile profile)
    {
      try
      {
        ActivePlayer.Intellect = profile.Intellect;
        ActivePlayer.Spirit = profile.Spirit;

        ActivePlayer.CritChance = (double)profile.CritPercent / 100;

        ActivePlayer.Haste = (double)profile.HastePercent / 100;
        ActivePlayer.Mastery = profile.Mastery;
      }
      catch
      {
        MessageBox.Show("Invalid player stats");
      }
    }

    private void UpdateEnemyValues(Profile profile)
    {
      ActiveEnemy.DamageModifier = (double)(profile.EnemyDamagePercent / 100);
    }

    private void UpdateHotKeyValues(Profile profile)
    {
      ActivePlayer.Abilities[typeof(AbilityAvengingWrath)].Hotkey.UpdateFromInputString(profile.KeyAW);
      ActivePlayer.Abilities[typeof(AbilityBeaconOfLight)].Hotkey.UpdateFromInputString(profile.KeyBOL);
      ActivePlayer.Abilities[typeof(AbilityDivineLight)].Hotkey.UpdateFromInputString(profile.KeyDL);
      ActivePlayer.Abilities[typeof(AbilityDivinePlea)].Hotkey.UpdateFromInputString(profile.KeyDP);
      ActivePlayer.Abilities[typeof(AbilityFlashOfLight)].Hotkey.UpdateFromInputString(profile.KeyFOL);

      ActivePlayer.Abilities[typeof(AbilityHolyLight)].Hotkey.UpdateFromInputString(profile.KeyHL);
      ActivePlayer.Abilities[typeof(AbilityHolyShock)].Hotkey.UpdateFromInputString(profile.KeyHS);
      ActivePlayer.Abilities[typeof(AbilityJudgment)].Hotkey.UpdateFromInputString(profile.KeyJudge);
      ActivePlayer.Abilities[typeof(AbilityLightOfDawn)].Hotkey.UpdateFromInputString(profile.KeyLOD);
      ActivePlayer.Abilities[typeof(AbilityWordOfGlory)].Hotkey.UpdateFromInputString(profile.KeyWOG);

      CancelKey.UpdateFromInputString(profile.KeyCancel);
    }

    /// <summary>
    /// Disable all periodic game updates
    /// </summary>
    public void StopUpdateTimer()
    {
      UpdateTimer.Stop();
    }

    /// <summary>
    /// Re-enable all periodic game updates
    /// </summary>
    public void ResumeUpdateTimer()
    {
      UpdateTimer.Start();
    }

  }
}
