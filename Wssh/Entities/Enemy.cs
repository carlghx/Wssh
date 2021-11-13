using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Wssh.Abilities.EnemyAbilities;
using Wssh.UI;
using Wssh.Buffs;

namespace Wssh.Entities
{
  /// <summary>
  /// Contains enemy buffs and abilities
  /// </summary>
  public class Enemy
  {
    private GameState _game;

    /// <summary>
    /// How many of the last casts to be displayed in the UI
    /// </summary>
    public const int NUM_CASTS_DISPLAY = 4;

    public const string DEFAULT_NAME = "Enemy";

    public const double DEFAULT_DAMAGE_MOD = 1.0;

    public double DamageModifier { get; set; }

    /// <summary>
    /// Display name used in combat log
    /// </summary>
    public string Name { get; set; }

    public ObservableCollection<LastCastUI> _lastCasts;
    /// <summary>
    /// List of the last abilities to be used
    /// </summary>
    public ObservableCollection<LastCastUI> LastCasts
    {
      get
      {
        if (_lastCasts == null)
          _lastCasts = new ObservableCollection<LastCastUI>();

        return _lastCasts;
      }
    }

    /// <summary>
    /// Add a cast to the enemy's display list
    /// </summary>
    /// <param name="targets">Party members affected by the spellcast</param>
    /// <param name="iconPath">Spellcast's icon</param>
    /// <param name="name">Spell's name</param>
    public void UpdateLastCasts(List<PartyMember> targets, string iconPath, string name)
    {
      if (name == AbilityMelee.NAME)
        return;

      LastCastUI newUI = new LastCastUI() { IconPath = iconPath, Message = name };
      if (targets.Count == 1)
        newUI.Target = targets[0].Name;

      LastCasts.Add(newUI);
      if (LastCasts.Count > NUM_CASTS_DISPLAY)
        LastCasts.RemoveAt(0);
    }


    private List<EnemyAbility> _abilities;
    /// <summary>
    /// List of abilities that the enemy uses
    /// </summary>
    public List<EnemyAbility> Abilities
    {
      get
      {
        return _abilities;
      }
      set
      {
        _abilities = value;
      }
    }

    public Enemy(GameState game)
    {
      _game = game;
      Name = DEFAULT_NAME;
      DamageModifier = DEFAULT_DAMAGE_MOD;
      InitSpellbook();
    }

    private void InitSpellbook()
    {
      Abilities = new List<EnemyAbility>();
      Abilities.Add(new AbilityMelee(_game));
      Abilities.Add(new AbilitySmash(_game));
      Abilities.Add(new AbilityWhirlwind(_game));
      Abilities.Add(new AbilityImpale(_game));
      Abilities.Add(new AbilityVolley(_game));
      Abilities.Add(new AbilitySWD(_game));
      Abilities.Add(new AbilityFrenzy(_game));
    }


    private ObservableCollection<Buff> _activeBuffs = new ObservableCollection<Buff>();

    public void AddBuff(Buff buff)
    {
      if (!buff.IsDebuff)
      {
        string message = "";
        message = String.Format("{0} gains {1}", Name, buff.Name);
        _game.LogMessage(message);
      }
      _activeBuffs.Add(buff);
    }


    public void RemoveBuff(Buff buff)
    {
      if (buff.IsDebuff)
      {
        string message = String.Format("{0} fades from {1}", buff.Name, buff.Target.Name);
        _game.LogMessage(message);
        buff.Target.StatusDebuff = "";
      }
      else
      {
        string message = "";
        message = String.Format("{0} loses {1}", Name, buff.Name);
        _game.LogMessage(message);

      }
      _activeBuffs.Remove(buff);
    }

    public Buff FindBuff(string name)
    {
      foreach (Buff buff in _activeBuffs)
      {
        if (buff.Name == name)
          return buff;
      }

      return null;
    }

    public void Tick(double timerIncrementSeconds)
    {
      TickBuffs(timerIncrementSeconds);
    }

    private void TickBuffs(double timerIncrementSeconds)
    {
      for (int i = 0; i < _activeBuffs.Count; )
      {
        Buff currentBuff = _activeBuffs[i];

        if (currentBuff.DurationRemaining != Buff.INFINITE_DURATION)
          currentBuff.DurationRemaining -= timerIncrementSeconds;

        TickTimerCheck(currentBuff, timerIncrementSeconds);
        TickDoTCheck(currentBuff);
        TickHoTCheck(currentBuff);

        if (currentBuff.DurationRemaining <= 0)
        {
          RemoveBuff(currentBuff);
        }
        else
          i++;
      }
    }

    private void TickTimerCheck(Buff currentBuff, double timerIncrementSeconds)
    {
      ITick tick = currentBuff as ITick;
      if (tick != null)
        tick.Timer += timerIncrementSeconds;
    }

    private void TickDoTCheck(Buff currentBuff)
    {
      IDoT dot = currentBuff as IDoT;
      if (dot != null)
      {
        if (dot.CheckTick())
        {
          int damage = dot.GetDamage(_game);
          string message = String.Format("{0} {1} {2} {3}", Name, currentBuff.Name, currentBuff.Target.Name, damage);
          _game.LogMessage(message);
          EnemyAbility.HandleDamage(currentBuff.Target, damage, _game);
        }
      }
    }

    private void TickHoTCheck(Buff currentBuff)
    {
      IHoT hot = currentBuff as IHoT;
      if (hot != null)
      {
        if (hot.CheckTick())
        {
          if (currentBuff.Target.IsAlive() && currentBuff.Target.Current != currentBuff.Target.Max)
          {
            int heal = hot.GetHeal();
            string message = String.Format("{0} {1} {2}", currentBuff.Name, currentBuff.Target.Name, heal);
            _game.LogMessage(message);
            currentBuff.Target.Current += heal;
          }
        }
      }
    }

    public void ResetStatus()
    {
      foreach (EnemyAbility ability in Abilities)
      {
        ability.CooldownRemaining = ability.Cooldown;
        if (!ability.UseImmediately)
          ability.CooldownRemaining += EnemyAbility.USE_DELAY_RANGE;
      }

      _activeBuffs.Clear();
      LastCasts.Clear();
    }

  }
}

