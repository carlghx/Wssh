using System;
using System.Windows;

using Wssh.UI;
using Wssh.Entities;
using Wssh.Abilities;
using Wssh.Buffs;

namespace Wssh.Utilities
{
  public class PlayerCastHandler
  {
    private GameState _game;

    /// <summary>
    /// the target being selected
    /// </summary>
    public PartyMember CurrentTarget { get; set; }

    /// <summary>
    /// the spell being selected
    /// </summary>
    public PlayerAbility CurrentSpell { get; set; }

    private Cast _castCurrent;
    /// <summary>
    /// The spell being cast
    /// </summary>
    public Cast CastCurrent
    {
      get
      {
        return _castCurrent;
      }
      private set
      {
        _castCurrent = value;
        if (_castCurrent == null)
          ClearCast();
        else
          SetCast();
      }
    }

    private void ClearCast()
    {
      _game.PlayerCastUI.Visible = Visibility.Collapsed;
    }

    private void SetCast()
    {      
      CastUI = _game.PlayerCastUI;
      CastUI.Visible = Visibility.Visible;
      CastUI.Current = 0;
      CastUI.DisplayLabel = _castCurrent.Ability.Name;
      CastUI.Max = _castCurrent.Ability.CastTime;

      PlayerAbility ability = (PlayerAbility)_castCurrent.Ability;
      if (ability.UsesGCD)
      {
        if (ability.ManaCost < _game.ActivePlayer.ManaCurrent && (!ability.UsesHolyPower || _game.ActivePlayer.HolyPower > 0))
          _game.TriggerGCD();
      }
    }


    private Cast _CastNext;
    /// <summary>
    /// The spell to cast after the current spell is completed
    /// </summary>
    public Cast CastNext
    {
      get
      {
        return _CastNext;
      }
      set
      {
        _CastNext = value;
      }
    }

    private BarUI _castUI;
    public BarUI CastUI
    {
      get
      {
        return _castUI;
      }
      set
      {
        _castUI = value;
      }
    }


    public PlayerCastHandler(GameState game)
    {
      _game = game;
    }

    public void Cancel()
    {
      CastCurrent = null;
      CastNext = null;
    }

    /// <summary>
    /// Attempt to cast a spell of the given type.
    /// If a spell is already being cast, set this spell as the next to be cast
    /// </summary>
    /// <param name="type">Type of spell to cast</param>
    public void UpdateCastAdd(Type type)
    {
      PlayerAbility ability = _game.ActivePlayer.Abilities[type];
      UpdateCastAdd(ability);
    }

    /// <summary>
    /// Attempt to cast a spell.
    /// If a spell is already being cast, set this spell as the next to be cast
    /// </summary>
    /// <param name="ability">Spell to cast</param>
    public void UpdateCastAdd(PlayerAbility ability)
    {

      if (CurrentTarget != null)
      {
        Cast cast = new Cast() { Ability = ability };

        if(ability.TargetType == AbilityTargetType.All)
          cast.Targets.AddRange(_game.ActiveParty.PartyMembers);
        else
          cast.Targets.Add(CurrentTarget);

        if (CastCurrent == null)
        {
          if (ability.CooldownRemaining > 0)
          {
            CastNext = cast;
            return;
          }

          if(_game.ActivePlayer.CanCast(ability))
            CastCurrent = cast;
        }
        else
          CastNext = cast;
      }
      else
      {
        // should always be someone targeted
      }
    }

    /// <summary>
    /// Update the current cast
    /// </summary>
    /// <param name="timerIncSeconds">Timer increment in seconds</param>
    public void UpdateTimer_Tick(double timerIncSeconds)
    {
      if (CastCurrent != null)
      {
        double hasteBonus = _game.ActivePlayer.Haste;
        Buff infusion = _game.ActivePlayer.FindBuff(BuffInfusion.NAME);
        if (infusion != null && BuffInfusion.IsInfusionAbility(CastCurrent.Ability))
        {
          double infusionHasteBonus = BuffInfusion.GetHasteBonus(AbilityHolyLight.CAST_TIME, _game);
          hasteBonus += infusionHasteBonus;
        }

        CastUI.Current += timerIncSeconds * (1 + hasteBonus);

        if (CastUI.Current >= CastUI.Max)
        {
          CompleteCurrentCast();
        }
      }
      else if (CastCurrent == null && CastNext != null)
      {
        PlayerAbility nextAbility = (PlayerAbility)CastNext.Ability;
        if (nextAbility.CooldownRemaining <= 0 && _game.ActivePlayer.CanCast(nextAbility))
        {
          CastCurrent = CastNext;
          CastNext = null;
        }
      }
    }

    private void CompleteCurrentCast()
    {
      ForceCast(CastCurrent);
    }

    /// <summary>
    /// Forces a given cast to happen, regardless of casting time or cooldown.
    /// Normal casting happens internally after UpdateCastAdd() and should not use this method.
    /// </summary>
    /// <param name="cast">Cast to force player u</param>
    public void ForceCast(Cast cast)
    {
      bool success = cast.ExecuteCast();
      if (success)
      {
        if (BuffInfusion.IsInfusionAbility(cast.Ability))
        {
          Buff infusion = _game.ActivePlayer.FindBuff(BuffInfusion.NAME);
          if (infusion != null)
            _game.ActivePlayer.RemoveBuff(infusion, _game);
        }

        if (cast.Ability.Cooldown > PlayerAbility.GCD)
        {
          PlayerAbility currentAbility = (PlayerAbility)cast.Ability;
          if (!_game.ActivePlayer.Cooldowns.Contains(currentAbility))
            _game.ActivePlayer.Cooldowns.Add(currentAbility);
        }
      }

      CastCurrent = null;
      if(CastNext != null)
        SetNextCast();
    }

    private void SetNextCast()
    {
      PlayerAbility nextAbility = (PlayerAbility)CastNext.Ability;
      if (_game.ActivePlayer.CanCast(nextAbility))
      {
        CastCurrent = CastNext;
        CastNext = null;
      }
    }

  }
}
