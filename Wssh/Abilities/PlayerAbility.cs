using System;

using Wssh.UI;
using Wssh.Entities;
using Wssh.Buffs;
using Wssh.Utilities;

namespace Wssh.Abilities
{
  /// <summary>
  /// Base class for a player-cast ability
  /// </summary>
  public abstract class PlayerAbility : Ability
  {    

    public const double GCD = 1.5;
    public const double MIN_GCD = 1.0;

    /// <summary>
    /// Whether this ability is on the global cooldown
    /// </summary>
    public bool UsesGCD { get; set; }

    public int ManaCost { get; set; }
    public bool UsesHolyPower { get; set; }

    /// <summary>
    /// Multiplier applied to a critical heal
    /// </summary>
    public const double CRIT_MULTIPLIER = 1.5;

    public HotkeyInfo Hotkey { get; set; }

    public string Description { get; set; }

    public readonly double POWER_MULTIPLIER;

    protected PlayerAbility(GameState game) : base(game)
    {

    }

    protected PlayerAbility(GameState game, int baseMin, int baseMax, double multiplier)
      : base(game, baseMin, baseMax)
    {
      POWER_MULTIPLIER = multiplier;
    }


    /// <summary>
    /// Adjusts opacity based on whether player has the mana to cast this ability
    /// </summary>
    public double OOMIndicator
    {
      get
      {
        if (UsesHolyPower && _game.ActivePlayer.HolyPower == 0)
          return OOMIndicatorUI.OPACITY_OOM;
        if (_game.ActivePlayer.ManaCurrent < ManaCost)
          return OOMIndicatorUI.OPACITY_OOM;

        return OOMIndicatorUI.OPACITY_NORMAL;
      }
    }   

    public int HolyPower
    {
      get
      {
        return _game.ActivePlayer.HolyPower;
      }
    }

    public int ManaCurrent
    {
      get
      {
        return _game.ActivePlayer.ManaCurrent;
      }
    }


    const double COOLDOWN_OPACITY_MAX = 0.75;
    const double COOLDOWN_OPACITY_MULTIPLIER = 1.5;
    public double Opacity
    {
      get
      {
        double percentRemaining = (CooldownRemaining) / Cooldown;
        double percentDisplay = percentRemaining;
        
        // amplify opacity effect so that it's more noticeable
        percentDisplay *= COOLDOWN_OPACITY_MULTIPLIER;

        // cap opacity to keep button from going completely black
        if (percentDisplay > COOLDOWN_OPACITY_MAX)
          percentDisplay = COOLDOWN_OPACITY_MAX;

        return percentDisplay;
      }
    }

    public void HandleTargetedHeal(PartyMember target, int amountHealed, bool crit)
    {
      HandleHeal(target, amountHealed, crit, true);
    }
    
    public void HandleAOEHeal(PartyMember target, int amountHealed, bool crit)
    {
      HandleHeal(target, amountHealed, crit, false);
    }

    public void HandleHeal(PartyMember target, int amountHealed, bool crit, bool isSingleTarget)
    {
      if (target.IsAlive())
        target.Current += amountHealed;

      if (isSingleTarget)
      {
        if (!target.PlayerCharacter)
          _game.ActivePlayer.ProtectorOfTheInnocent();
      }

      if (crit)
      {
        _game.ActivePlayer.AddMasteryBuff(target, amountHealed, _game);
        _game.ActivePlayer.AddConvictionBuff(_game);
      }

      CheckBeaconOfLight(target, amountHealed, _game);
    }

    /// <summary>
    /// Check for Beacon of Light heal after doing a main heal
    /// </summary>
    /// <param name="target"></param>
    /// <param name="amountHealed"></param>
    public static void CheckBeaconOfLight(PartyMember target, int amountHealed, GameState game)
    {
      Buff beacon = game.ActivePlayer.FindBuff(AbilityBeaconOfLight.NAME);
      if (beacon != null)
      {
        HandleBeaconOfLight(beacon.Target, target, amountHealed, game);
      }
    }

    /// <summary>
    /// Check for Beacon of Light heal after doing a main heal
    /// </summary>
    /// <param name="beaconTarget">Party member with Beacon of Light</param>
    /// <param name="mainTarget">Party member that was targeted by original heal</param>
    /// <param name="amountHealed">Amount healed on main target</param>
    private static void HandleBeaconOfLight(PartyMember beaconTarget, PartyMember mainTarget, int amountHealed, GameState game)
    {

      if (beaconTarget.IsAlive() && beaconTarget != mainTarget)
      {
        int amountBeaconHealed = (int)(amountHealed * AbilityBeaconOfLight.HEAL_MODIFIER);
        string message = String.Format("{0} {1} {2} {3}", game.ActivePlayer.Name, AbilityBeaconOfLight.NAME, beaconTarget.Name, amountBeaconHealed);
        game.LogMessage(message);
        beaconTarget.Current += amountBeaconHealed;
      }
    }

    /// <summary>
    /// Apply modifiers to a successfully cast heal
    /// </summary>
    /// <param name="crit">Whether the heal is a critical</param>
    /// <param name="baseHeal">Base amount healed</param>
    /// <returns>Modified heal amount</returns>
    public int AddHealModifiers(bool crit, double baseHeal)
    {
      return AddHealModifiers(crit, baseHeal, _game);
    }

    /// <summary>
    /// Apply modifiers to a successfully cast heal
    /// </summary>
    /// <param name="crit">Whether the heal is a critical</param>
    /// <param name="baseHeal">Base amount healed</param>
    /// <param name="game"></param>
    /// <returns>Modified heal amount</returns>
    public static int AddHealModifiers(bool crit, double baseHeal, GameState game)
    {
      double newHeal = baseHeal;
      newHeal = newHeal * game.ActivePlayer.ConvictionMultiplier();
      newHeal = newHeal * Player.HEALING_MULTIPLIER;

      if (crit)
        newHeal = newHeal * CRIT_MULTIPLIER;

      Buff divinePlea = game.ActivePlayer.FindBuff(BuffDivinePlea.NAME);
      if (divinePlea != null)
        newHeal = newHeal * BuffDivinePlea.HEAL_MODIFIER;

      Buff avengingWrath = game.ActivePlayer.FindBuff(BuffAvengingWrath.NAME);
      if (avengingWrath != null)
        newHeal = newHeal * (BuffAvengingWrath.MODIFIER);

      return (int)newHeal;
    }

    protected void CheckTowerOfRadiance(PartyMember target)
    {
      Buff beaconBuff = _game.ActivePlayer.FindBuff(AbilityBeaconOfLight.NAME);
      if (beaconBuff != null && beaconBuff.Target == target)
      {
        string message = String.Format("{0} gain 1 Holy Power from Tower of Radiance", _game.ActivePlayer.Name);
        _game.LogMessage(message);
        _game.ActivePlayer.HolyPower++;
      }
    }

    protected void CheckProcDaybreak()
    {
      Random random = App.RandomGen;
      if (random.NextDouble() < BuffDaybreak.PROC_CHANCE)
      {
        Buff daybreakBuff = new BuffDaybreak(_game.ActivePlayer.PartyFrameUI);
        _game.ActivePlayer.AddOrRefreshBuff(daybreakBuff, _game);
      }
    }
    
    protected void LogTargetedHeal(PartyMember target, int amountHealed, bool crit = false)
    {
      string message = String.Format("{0} {1} {2} {3}", _game.ActivePlayer.Name, Name, target.Name, amountHealed);
      if (crit)
        message = message + " (Crit)";
      _game.LogMessage(message);
    }

    protected void LogNonHeal(bool crit = false)
    {
      string message = String.Format("{0} {1}", _game.ActivePlayer.Name, Name);
      if (crit)
        message = message + " (Crit)";
      _game.LogMessage(message);
    }

  }
}