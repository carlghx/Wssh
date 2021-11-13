using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Wssh.UI;
using Wssh.Utilities;
using Wssh.Entities;
using Wssh.Abilities;
using Wssh.Abilities.EnemyAbilities;
using Wssh.Buffs;


namespace Wssh
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private GameState _game;

    public const string DEFAULT_CANCEL_KEY = "E";
    /// <summary>
    /// Key used to cancel the current cast
    /// </summary>
    public static HotkeyInfo CANCEL_KEY = HotkeyInfo.CreateFromKeyString(DEFAULT_CANCEL_KEY);


    public MainWindow()
    {
      InitializeComponent();

      InitializeFirstTime();
      SetupWindow();
    }

    /// <summary>
    /// Initialization that is only needed the first time the application is started
    /// </summary>
    private void InitializeFirstTime()
    {
      _game = new GameState(combatLog);

      InitUI(_game.ActivePlayer);
    }
    
    /// <summary>
    /// Initialization that has to be called after every reset or game settings change
    /// </summary>
    public void SetupWindow()
    {
      _game.SetupGame();

      CheckEnemyCastOpacity();

      InitSpellBar();

      _game.ActivePlayer.BindBuffsToList(listBuffs);
      listLastEnemyCast.ItemsSource = _game.ActiveEnemy.LastCasts;
      listCooldowns.ItemsSource = _game.ActivePlayer.Cooldowns;

      partyFrame.SelectedIndex = 0;
    }


    private void StartCombatWindow()
    {
      if (partyFrame.SelectedIndex == -1)
        partyFrame.SelectedIndex = 0;

      _game.StartCombatGame();
      CheckEnemyCastOpacity();
    }

    private void StopCombatWindow()
    {
      _game.StopCombatGame();
      CheckEnemyCastOpacity();
    }

    private void InitSpellBar()
    {
      spellBtnWOG.DataContext = _game.ActivePlayer.Abilities[typeof(AbilityWordOfGlory)];
      spellBtnWOG.BindOOMToHolyPower(_game);
      spellBtnFOL.DataContext = _game.ActivePlayer.Abilities[typeof(AbilityFlashOfLight)];
      spellBtnFOL.BindOOMToSpell(_game);
      spellBtnDL.DataContext = _game.ActivePlayer.Abilities[typeof(AbilityDivineLight)];
      spellBtnDL.BindOOMToSpell(_game);
      spellBtnHL.DataContext = _game.ActivePlayer.Abilities[typeof(AbilityHolyLight)];
      spellBtnHL.BindOOMToSpell(_game);
      spellBtnBeacon.DataContext = _game.ActivePlayer.Abilities[typeof(AbilityBeaconOfLight)];
      spellBtnHS.DataContext = _game.ActivePlayer.Abilities[typeof(AbilityHolyShock)];
      spellBtnHS.BindOOMToSpell(_game);
      spellBtnJudgment.DataContext = _game.ActivePlayer.Abilities[typeof(AbilityJudgment)];
      spellBtnJudgment.BindOOMToSpell(_game);
      spellBtnLightOfDawn.DataContext = _game.ActivePlayer.Abilities[typeof(AbilityLightOfDawn)];
      spellBtnLightOfDawn.BindOOMToHolyPower(_game);
      spellBtnPlea.DataContext = _game.ActivePlayer.Abilities[typeof(AbilityDivinePlea)];
      spellBtnAvengingWrath.DataContext = _game.ActivePlayer.Abilities[typeof(AbilityAvengingWrath)];
      spellBtnAvengingWrath.BindOOMToSpell(_game);
    }

    private void InitUI(Player player)
    {
      partyFrame.ItemsSource = _game.ActiveParty.PartyMembers;

      barMana.UI = player.PlayerManaUI;      
      barHoly.UI = player.HolyPowerUI;
      barCast.UI = _game.PlayerCastUI;
            
      stkLastCast.DataContext = player.LastCast;

      this.DataContext = _game.ConsoleUI;
      combatLog.Console = _game.ConsoleUI;
    }


    private void spellBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      SpellButton sb = (SpellButton)sender;
      PlayerAbility ability = (PlayerAbility)sb.DataContext;

      _game.UseAbility(ability);
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
      Key k = e.Key;
      string keyStr = k.ToString();

      _game.HandleKeyStringDown(keyStr);
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      Key k = e.Key;
      string keyStr = k.ToString();

      _game.HandleKeyStringUp(keyStr);
    }

    private void pFrame_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ListView list = (ListView)sender;
      PartyMember currentTarget = (PartyMember)list.SelectedItem;

      _game.SetSelectedPartyMember(currentTarget);
    }

    private void pFrame_Loaded(object sender, RoutedEventArgs e)
    {
      partyFrame.SelectedIndex = 0;
    }

    private void btnStart_Click(object sender, RoutedEventArgs e)
    {
      SetupWindow();
      StartCombatWindow();
    }

    private void DI_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      MessageBoxResult result = MessageBox.Show("Reset encounter?", "Divine Intervention", MessageBoxButton.YesNo);
      if (result == MessageBoxResult.Yes)
      {
        StopCombatWindow();
      }
    }

    private void btnSettings_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        SettingsWindow settings = new SettingsWindow(_game);

        bool? dialogOK = settings.ShowDialog();

        if (dialogOK != null && dialogOK == true)
        {
          _game.UpdateFromProfile(settings.CurrentProfile);
          SetupWindow();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error loading profile: " + ex);
        StorageHandler.WriteError(ex.ToString());
      }
    }

    private void btnLog_Click(object sender, RoutedEventArgs e)
    {
      if (combatLog.Visibility == Visibility.Collapsed)
      {
        ShowCombatLog();
      }
      else
      {
        HideCombatLog();
      }

      CheckEnemyCastOpacity();
    }

    private void ShowCombatLog()
    {
      combatLog.Visibility = Visibility.Visible;
      btnLog.Content = "Hide Log";
    }

    private void HideCombatLog()
    {
      combatLog.Visibility = Visibility.Collapsed;
      btnLog.Content = "Show Log";
    }

    private void CheckEnemyCastOpacity()
    {

      if (!_game.ConsoleUI.InCombat || combatLog.Visibility == Visibility.Visible)        
        listLastEnemyCast.Opacity = LastCastUI.OPACITY_FADE;
      else
        listLastEnemyCast.Opacity = 1.0;
    }

  }
}
