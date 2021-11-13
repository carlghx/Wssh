using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

using Wssh.Abilities;
using Wssh.UI;
using Wssh.Buffs;

namespace Wssh.Entities
{

  /// <summary>
  /// Contains player's buffs, abilities, and stats
  /// </summary>
  public class Player
  {
    private GameState _game;
    private OOMIndicatorUI _oomUI;

    public const double DEFAULT_CRIT = 0.15;
    public const double DEFAULT_HASTE = 0.20;
    public const int DEFAULT_INT = 4800;
    //public const int DEFAULT_INT = 1;
    public const int DEFAULT_SPI = 1800;
    public const int DEFAULT_MASTERY = 5;
    public const string DEFAULT_NAME = "You";

    /// <summary>
    /// Name used for player in party frame and combat log
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Healing bonus for Holy Specialization, Divinity, Seal of Insight
    /// </summary>
    public static double HEALING_MULTIPLIER = 1.21;

    /// <summary>
    /// Spellpower bonus from weapon itemization
    /// </summary>
    public static int WEAPON_SPELLPOWER = 1729;

    /// <summary>
    /// Spellpower multiplier from buff
    /// </summary>
    public static double SPELLPOWER_BUFF = 1.06;

    /// <summary>
    /// Mana bonus from Arcane intellect
    /// </summary>
    public static int ARCANE_INTELLECT_BONUS = 2126;

    public LastCastUI LastCast { get; set; }
    public BarUI PlayerManaUI { get; set; }
    public BarUI HolyPowerUI { get; set; }

    /// <summary>
    /// The party health bar that represents the player
    /// </summary>
    public PartyMember PartyFrameUI { get; set; }

    /// <summary>
    /// Player's spellpower
    /// </summary>
    public int SpellPower
    {
      get
      {
        int basePower = Intellect + WEAPON_SPELLPOWER;
        return (int)(basePower * SPELLPOWER_BUFF);
      }
    }

    /// <summary>
    /// Adjust global cooldown for haste
    /// </summary>
    /// <returns>Player's global cooldown with current haste effects</returns>
    public double GetModifiedGCD()
    {
      double modGCD = PlayerAbility.GCD * HasteMultiplier();
      if (modGCD < PlayerAbility.MIN_GCD)
        modGCD = PlayerAbility.MIN_GCD;

      return modGCD;
    }

    private int _int;
    /// <summary>
    /// Intellect increases spellpower and maximum mana
    /// </summary>
    public int Intellect
    {
      get
      {
        return _int;
      }
      set
      {
        _int = value;
        PlayerManaUI.Max = ManaMax;
        PlayerManaUI.OnPropertyChanged(BarUI.PropertyStringMax);
        PlayerManaUI.OnPropertyChanged(BarUI.PropertyStringCurrent);
      }
    }

    /// <summary>
    /// Spirit increases mana regeneration
    /// </summary>
    public int Spirit { get; set; }

    /// <summary>
    /// Player's Mastery points (not rating)
    /// Improves the shield from Illuminated Healing
    /// </summary>
    public int Mastery { get; set; }

    private double _haste;
    /// <summary>
    /// Player's haste: 0.20 = 20% haste.
    /// Lower casting time and global cooldown
    /// </summary>
    public double Haste
    {
      get
      {
        Buff buff = FindBuff(BuffJudgment.NAME);
        if (buff != null)
          return _haste + BuffJudgment.HASTE;
        return _haste;
      }
      set
      {
        _haste = value;
      }
    }

    public double HasteMultiplier()
    {
      return 1.0 / (1.0 + Haste);
    }

    /// <summary>
    /// Player's spell critical chance: 0.20 = 20% critical chance.
    /// </summary>
    public double CritChance { get; set; }

    /// <summary>
    /// Amount of mana regenerated in 1 second of game time
    /// </summary>
    public double ManaRegenPerSecond
    {
      get
      {
        return 0.0005 + (Spirit * Math.Sqrt(Intellect) * 0.0083625);
      }
    }

    /// <summary>
    /// Base mana for a level 85 character, before stats
    /// </summary>
    public int ManaBase = 23142;

    public int ManaCurrent
    {
      get
      {
        if (PlayerManaUI != null)
          return (int)PlayerManaUI.Current;

        return 0;
      }
      set
      {
        if (PlayerManaUI != null)
        {
          PlayerManaUI.Current = value;
          _oomUI.Mana = value;
        }
      }
    }

    /// <summary>
    /// Calculated mana capacity, after stats and buffs
    /// </summary>
    public int ManaMax
    {
      get
      {
        return Intellect * 15 + ManaBase + ARCANE_INTELLECT_BONUS;
      }
    }


    private Dictionary<Type, PlayerAbility> _abilities;
    public Dictionary<Type, PlayerAbility> Abilities
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

    public Player(GameState game, OOMIndicatorUI oomUI)
    {
      _game = game;
      _oomUI = oomUI;
      Name = DEFAULT_NAME;

      PlayerManaUI = BarUI.CreateManaUI();
      HolyPowerUI = BarUI.CreateHolyUI();
      LastCast = new LastCastUI();
      PartyFrameUI = PartyMember.MakePaladinPlayer(Name);

      CritChance = DEFAULT_CRIT;
      Haste = DEFAULT_HASTE;
      Intellect = DEFAULT_INT;
      Spirit = DEFAULT_SPI;
      Mastery = DEFAULT_MASTERY;      
      
      PlayerManaUI.Max = ManaMax;
      PlayerManaUI.Current = ManaMax;

      InitSpellBook();
    }

    /// <summary>
    /// Reset all buffs and cooldowns, restore mana, and remove holy power
    /// </summary>
    public void ResetStatus()
    {
      foreach (PlayerAbility ability in Abilities.Values)
      {
        ability.CooldownRemaining = 0;
      }

      _activeBuffs.Clear();
      Cooldowns.Clear();
      ManaCurrent = ManaMax;
      HolyPower = 0;
      LastCast.Clear();
    }

    private void InitSpellBook()
    {
      Abilities = new Dictionary<Type, PlayerAbility>();

      Abilities.Add(typeof(AbilityAvengingWrath), new AbilityAvengingWrath(_game));
      Abilities.Add(typeof(AbilityHolyLight), new AbilityHolyLight(_game));
      Abilities.Add(typeof(AbilityBeaconOfLight), new AbilityBeaconOfLight(_game));
      Abilities.Add(typeof(AbilityHolyShock), new AbilityHolyShock(_game));
      Abilities.Add(typeof(AbilityWordOfGlory), new AbilityWordOfGlory(_game));
      Abilities.Add(typeof(AbilityFlashOfLight), new AbilityFlashOfLight(_game));
      Abilities.Add(typeof(AbilityDivineLight), new AbilityDivineLight(_game));
      Abilities.Add(typeof(AbilityJudgment), new AbilityJudgment(_game));
      Abilities.Add(typeof(AbilityLightOfDawn), new AbilityLightOfDawn(_game));
      Abilities.Add(typeof(AbilityDivinePlea), new AbilityDivinePlea(_game));      
    }

    public const int PROTECTOR_OF_INNOCENT_MIN = 2605;
    public const int PROTECTOR_OF_INNOCENT_MAX = 2997;
    public const double PROTECTOR_OF_INNOCENT_MULTIPLIER = 0.067;
    /// <summary>
    /// A self-heal that is triggered after healing an ally
    /// </summary>
    public void ProtectorOfTheInnocent()
    {
      Random random = App.RandomGen;
      int amountHealed = random.Next(PROTECTOR_OF_INNOCENT_MIN, PROTECTOR_OF_INNOCENT_MAX);
      amountHealed += (int)(SpellPower * PROTECTOR_OF_INNOCENT_MULTIPLIER);

      PartyFrameUI.Current += amountHealed;
      string message = String.Format("{0} Protector of the Innocent {1}", _game.ActivePlayer.Name, amountHealed);
      _game.LogMessage(message);

      PlayerAbility.CheckBeaconOfLight(PartyFrameUI, amountHealed, _game);
    }

    /// <summary>
    /// Private collection; use AddBuff, RemoveBuff, and BindBuffsToList to access from outside
    /// </summary>
    private ObservableCollection<Buff> _activeBuffs = new ObservableCollection<Buff>();

    /// <summary>
    /// Adds a buff to the ActiveBuffs list
    /// </summary>
    /// <param name="buff">Buff to be added</param>
    public void AddBuff(Buff buff, GameState game)
    {
      string message = buff.GetLogMessage(game);
      _game.LogMessage(message);
      _activeBuffs.Add(buff);
    }

    /// <summary>
    /// Adds a buff to the ActiveBuffs list and removes any old instances of the same buff
    /// </summary>
    /// <param name="buff">Buff to be added</param>
    public void AddUniqueBuff(Buff buff, GameState game)
    {
      Buff currentBuff = FindBuff(buff.Name);
      if (currentBuff != null)
      {
        currentBuff.Target.StatusBuff = "";
        RemoveBuff(currentBuff, game);
      }

      buff.Target.StatusBuff = buff.IconPath;
      AddBuff(buff, game);
    }

    /// <summary>
    /// Adds a buff to the ActiveBuffs list, or refreshes duration if that buff is already present
    /// </summary>
    /// <param name="buff">Buff to be added or refreshed</param>
    /// <returns>The new or refreshed buff</returns>
    public Buff AddOrRefreshBuff(Buff buff, GameState game)
    {
      Buff currentBuff = FindBuff(buff.Name);
      if (currentBuff != null)
      {
        currentBuff.DurationRemaining = buff.DurationRemaining;
        return currentBuff;
      }
      else
      {
        AddBuff(buff, game);
        return buff;
      }
    }

    /// <summary>
    /// Create shield on target of a critical heal
    /// </summary>
    /// <param name="target"></param>
    /// <param name="amountHealed"></param>
    public void AddMasteryBuff(PartyMember target, int amountHealed, GameState game)
    {
      double shieldMultiplier = 0.1 + (Mastery * 0.0125);
      int shieldStrength = (int)(amountHealed * shieldMultiplier);
      
      Buff newBuff = new BuffIlluminatedHealing(target, shieldStrength);
      
      Buff currentBuff = AddOrRefreshBuff(newBuff, game);
      if (currentBuff.Amount < shieldStrength)
      {
        currentBuff.Amount = shieldStrength;
      }
    }

    /// <summary>
    /// Increases healing by 3% on critical. Stacks.
    /// </summary>
    public void AddConvictionBuff(GameState game)
    {
      Buff newBuff = new BuffConviction(game.ActivePlayer.PartyFrameUI);

      Buff currentBuff = AddOrRefreshBuff(newBuff, game);
      if (currentBuff.Amount < BuffConviction.MAX_STACKS)
        currentBuff.Amount++;

    }

    /// <summary>
    /// Multiplier to healing provided by Conviction buff
    /// </summary>
    /// <returns></returns>
    public double ConvictionMultiplier()
    {
      double mult = 1.0;
      Buff currentBuff = FindBuff(BuffConviction.NAME);
      if (currentBuff != null)
      {
        mult += currentBuff.Amount * BuffConviction.CONVICTION_MULTIPLIER;
      }

      return mult;
    }

    public void RemoveBuff(Buff buff, GameState game)
    {
      string message = String.Format("{0} lose {1}", game.ActivePlayer.Name, buff.Name);
      _game.LogMessage(message);
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

    public Buff FindBuffOnTarget(string name, PartyMember target)
    {
      foreach (Buff buff in _activeBuffs)
      {
        if (buff.Name == name && buff.Target == target)
          return buff;
      }

      return null;
    }

    public int HolyPower
    {
      get
      {
        return (int)HolyPowerUI.Current;
      }
      set
      {
        HolyPowerUI.Current = value;
        _oomUI.HolyPower = value;
      }
    }

    private ObservableCollection<PlayerAbility> _cooldowns;
    public ObservableCollection<PlayerAbility> Cooldowns
    {
      get
      {
        if (_cooldowns == null)
          _cooldowns = new ObservableCollection<PlayerAbility>();

        return _cooldowns;
      }
      set
      {
        _cooldowns = value;
      }
    }

    public void Tick(double timerIncrementSeconds, GameState game)
    {
      TickMana(timerIncrementSeconds, game);
      TickBuffs(timerIncrementSeconds, game);
      TickCooldowns(timerIncrementSeconds, game);
    }

    private void TickMana(double timerIncrementSeconds, GameState game)
    {
      ManaCurrent += (int)(ManaRegenPerSecond * timerIncrementSeconds);
    }

    private void TickBuffs(double timerIncrementSeconds, GameState game)
    {
      for (int i = 0; i < _activeBuffs.Count; )
      {
        Buff currentBuff = _activeBuffs[i];
        currentBuff.DurationRemaining -= timerIncrementSeconds;

        BuffDivinePlea plea = currentBuff as BuffDivinePlea;
        if (plea != null && plea.CheckTick())
        {
          ManaCurrent += (int)(ManaMax * BuffDivinePlea.MANA_PER_TICK);
        }

        if (currentBuff.DurationRemaining <= 0)
          RemoveBuff(currentBuff, game);
        else
          i++;
      }

    }

    private void TickCooldowns(double timerIncrementSeconds, GameState game)
    {
      foreach (KeyValuePair<Type, PlayerAbility> pair in Abilities)
      {
        PlayerAbility ability = pair.Value;
        if (ability.CooldownRemaining > 0)
          ability.CooldownRemaining -= timerIncrementSeconds;
      }

      for (int i = 0; i < Cooldowns.Count; )
      {
        PlayerAbility currentAbility = Cooldowns[i];
        if (currentAbility.CooldownRemaining <= 0)
          Cooldowns.Remove(currentAbility);
        else
          i++;
      }
    }

    /// <summary>
    /// Whether the player has the resources (mana/holy power) to cast an ability
    /// </summary>
    /// <param name="ability">Ability to check</param>
    /// <returns>True if player is able to cast; false otherwise</returns>
    public bool CanCast(PlayerAbility ability)
    {
      if (ability.ManaCost > _game.ActivePlayer.ManaCurrent)
        return false;

      if (ability.UsesHolyPower && _game.ActivePlayer.HolyPower == 0)
        return false;

      return true;
    }

    /// <summary>
    /// Bind the player's active buffs to an ItemsControl UI element
    /// </summary>
    /// <param name="listBuffs">ItemsControl to bind the buffs to</param>
    public void BindBuffsToList(System.Windows.Controls.ItemsControl listBuffs)
    {
      listBuffs.ItemsSource = _activeBuffs;
    }
  }  

}
