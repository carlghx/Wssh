using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Wssh.Entities;
using Wssh.Abilities;
using Wssh.Abilities.EnemyAbilities;
using Wssh.Utilities;

namespace Wssh
{
  /// <summary>
  /// Interaction logic for SettingsWindow.xaml
  /// </summary>
  public partial class SettingsWindow : Window
  {
    private GameState _game;
    
    public Profile CurrentProfile { get; set; }

    private HotkeyBox _focusedHotkey;

    public List<HotkeyBox> Hotkeys { get; set; }

    private void InitHotkeyControls()
    {
      Hotkeys = new List<HotkeyBox>();
      Hotkeys.Add(keyAW);
      Hotkeys.Add(keyBOL);
      Hotkeys.Add(keyDL);
      Hotkeys.Add(keyDP);
      Hotkeys.Add(keyFOL);

      Hotkeys.Add(keyHL);
      Hotkeys.Add(keyHS);
      Hotkeys.Add(keyJudge);
      Hotkeys.Add(keyLOD);
      Hotkeys.Add(keyWOG);
      Hotkeys.Add(keyCancel);

      foreach (HotkeyBox hotkeyControl in Hotkeys)
      {
        hotkeyControl.ParentSettings = this;
      }
    }

    public void HotkeyFocused(HotkeyBox focused)
    {
      _focusedHotkey = focused;
      foreach (HotkeyBox hotkeyControl in Hotkeys)
      {
        if (hotkeyControl != focused)
        {
          hotkeyControl.UnFocus();
        }
      }

      focused.Focus();
    }

    private void HotkeyUnFocusAll()
    {
      _focusedHotkey = null;
      foreach (HotkeyBox hotkeyControl in Hotkeys)
      {
        hotkeyControl.UnFocus();
      }
    }

    public SettingsWindow(GameState game)
    {
      InitializeComponent();
      _game = game;
      CurrentProfile = Profile.LoadProfileFromGame(game);
      this.DataContext = CurrentProfile;

      InitHotkeyControls();

      LoadSettingsFromCurrent();      
    }

    private void LoadSettingsFromCurrent()
    {
      LoadPlayerSettingsFromCurrent();
      LoadEnemySettingsFromCurrent();
      LoadHotkeySettingsFromCurrent();
    }

    public void LoadPlayerSettingsFromCurrent()
    {
      numInt.Value = CurrentProfile.Intellect;
      numSpi.Value = CurrentProfile.Spirit;
      numCrit.Value = CurrentProfile.CritPercent;
      numHaste.Value = CurrentProfile.HastePercent;
      numMaster.Value = CurrentProfile.Mastery;
    }

    public void LoadEnemySettingsFromCurrent()
    {
      numEnemy.Value = CurrentProfile.EnemyDamagePercent;
    }

    private void LoadHotkeySettingsFromCurrent()
    {
      keyAW.Text = CurrentProfile.KeyAW;
      keyBOL.Text = CurrentProfile.KeyBOL;
      keyDL.Text = CurrentProfile.KeyDL;
      keyDP.Text = CurrentProfile.KeyDP;
      keyFOL.Text = CurrentProfile.KeyFOL;

      keyHL.Text = CurrentProfile.KeyHL;
      keyHS.Text = CurrentProfile.KeyHS;
      keyJudge.Text = CurrentProfile.KeyJudge;
      keyLOD.Text = CurrentProfile.KeyLOD;
      keyWOG.Text = CurrentProfile.KeyWOG;
      keyCancel.Text = CurrentProfile.KeyCancel;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      this.Close();
    }

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      this.Close();
    }

    private void btnDefault_Click(object sender, RoutedEventArgs e)
    {
      numInt.Value = Player.DEFAULT_INT;
      numSpi.Value = Player.DEFAULT_SPI;
      numCrit.Value = (int)(Player.DEFAULT_CRIT * 100);
      numHaste.Value = (int)(Player.DEFAULT_HASTE * 100);
      numMaster.Value = Player.DEFAULT_MASTERY;

      numEnemy.Value = (int)(Enemy.DEFAULT_DAMAGE_MOD * 100);

      keyAW.Text = AbilityAvengingWrath.DEFAULT_KEY;
      keyBOL.Text = AbilityBeaconOfLight.DEFAULT_KEY;
      keyDL.Text = AbilityDivineLight.DEFAULT_KEY;
      keyDP.Text = AbilityDivinePlea.DEFAULT_KEY;
      keyFOL.Text = AbilityFlashOfLight.DEFAULT_KEY;

      keyHL.Text = AbilityHolyLight.DEFAULT_KEY;
      keyHS.Text = AbilityHolyShock.DEFAULT_KEY;
      keyJudge.Text = AbilityJudgment.DEFAULT_KEY;
      keyLOD.Text = AbilityLightOfDawn.DEFAULT_KEY;
      keyWOG.Text = AbilityWordOfGlory.DEFAULT_KEY;

      keyCancel.Text = MainWindow.DEFAULT_CANCEL_KEY;
    }

    /// <summary>
    /// Load a profile from file
    /// </summary>
    /// <param name="p"></param>
    private void LoadFromProfile(Profile p)
    {
      CurrentProfile = p;
      this.DataContext = CurrentProfile;

      LoadSettingsFromCurrent();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      string name = txtFile.Text.Trim();
      if (name == "")
      {
        MessageBox.Show("You must name your profile", "Save Profile");
        return;
      }

      if (StorageHandler.ProfileExists(name))
      {
        MessageBoxResult result = MessageBox.Show("Profile already exists, overwrite?", "Confirm save", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.No)
          return;
      }

      bool success = StorageHandler.SaveProfile(CurrentProfile);
      if (success)
        MessageBox.Show("Profile saved successfully in " + App.AppDirectory + name + ".wssh", "Save Profile");
      else
        MessageBox.Show("Error saving profile", "Save Profile");
    }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
      string name = txtFile.Text.Trim();
      if (name == "")
      {
        MessageBox.Show("You must name your profile", "Load Profile");
        return;
      }

      if (!StorageHandler.ProfileExists(name))
      {
        MessageBox.Show("Profile not found: " + name, "Load Profile");
        return;
      }

      Profile p = StorageHandler.Load(name);
      if (p != null)
      {
        MessageBox.Show("Profile loaded successfully", "Load Profile");
        LoadFromProfile(p);
      }
      else
        MessageBox.Show("Error loading profile", "Load Profile");

    }

    private void key_GotFocus(object sender, RoutedEventArgs e)
    {
      TextBox textBox = (TextBox)sender;
      textBox.SelectAll();
    }

    private void Hotkey_KeyDown(object sender, KeyEventArgs e)
    {
      string keyStr = e.Key.ToString();
      if (keyStr == KeyModHandler.LEFT_SHIFT || keyStr == KeyModHandler.RIGHT_SHIFT)
        _game.KeyHandler.ShiftDown = true;
      else if (keyStr == KeyModHandler.LEFT_CTRL || keyStr == KeyModHandler.RIGHT_CTRL)
        _game.KeyHandler.CtrlDown = true;

      else if (_focusedHotkey != null)
      {
        if (keyStr == KeyModHandler.TAB)
        {
          HotkeyUnFocusAll();
        }
        else if (KeyModHandler.InvalidKeyStr(keyStr))
        {
          return;
        }
        else if (KeyModHandler.ArrowKeyStr(keyStr))
        {
          HotkeyUnFocusAll();
        }
        else
        {
          _focusedHotkey.Text = e.Key.ToString();
          KeyModHandler keyHandler = _game.KeyHandler;
          _focusedHotkey.Text = HotkeyInfo.GetDisplayString(_focusedHotkey.Text, keyHandler.ShiftDown, keyHandler.CtrlDown);
        }
      }
    }

    private void grdHotkey_MouseLeave(object sender, MouseEventArgs e)
    {
      HotkeyUnFocusAll();
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      string keyStr = e.Key.ToString();
      if (keyStr == KeyModHandler.LEFT_SHIFT || keyStr == KeyModHandler.RIGHT_SHIFT)
        _game.KeyHandler.ShiftDown = false;
      else if (keyStr == KeyModHandler.LEFT_CTRL || keyStr == KeyModHandler.RIGHT_CTRL)
        _game.KeyHandler.CtrlDown = false;
    }

  }
}
