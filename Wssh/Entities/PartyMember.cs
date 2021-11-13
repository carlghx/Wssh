using System.Windows.Media;

using Wssh.UI;
using Wssh.Utilities;

namespace Wssh.Entities
{
  /// <summary>
  /// Binds to party health bars
  /// Modified bar UI with additional properties
  /// </summary>
  public class PartyMember : BarUI
  {
    public const int HEALTH_BAR_HEIGHT = 60;
    public const int HEALTH_BAR_WIDTH = 250;

    public string Name
    {
      get
      {
        return Label;
      }
    }

    /// <summary>
    /// Whether this party member is considered Melee for targeting purposes
    /// </summary>
    public bool Melee { get; set; }

    /// <summary>
    /// Whether this party member is considered a Tank for targeting purposes
    /// </summary>
    public bool Tank { get; set; }

    /// <summary>
    /// Whether this party member is eligible for Shadow Priest-based buffs and abilities
    /// </summary>
    public bool Shadow { get; set; }

    /// <summary>
    /// Whether this party member is the player character
    /// </summary>
    public bool PlayerCharacter { get; set; }

    public string PropertyStringStatusDebuff = MemberNameFinder<PartyMember>.GetMemberName(x => x.StatusDebuff);
    private string _statusDebuff;
    /// <summary>
    /// Icon for displaying current debuff.
    /// Only supports one debuff at a time.
    /// </summary>
    public string StatusDebuff
    {
      get
      {
        return _statusDebuff;
      }
      set
      {
        _statusDebuff = value;
        OnPropertyChanged(PropertyStringStatusDebuff);
      }
    }

    public string PropertyStringStatusBuff = MemberNameFinder<PartyMember>.GetMemberName(x => x.StatusBuff);
    private string _statusBuff;
    /// <summary>
    /// Icon path for displaying current buff.
    /// Only supports one buff at a time.
    /// </summary>
    public string StatusBuff
    {
      get
      {
        return _statusBuff;
      }
      set
      {
        _statusBuff = value;
        OnPropertyChanged(PropertyStringStatusBuff);
      }
    }

    /// <summary>
    /// Whether this party member is alive.
    /// Most abilities will not work on dead party members.
    /// </summary>
    /// <returns>True if party member's health is > 0; false otherwise</returns>
    public bool IsAlive()
    {
      return Current > 0;
    }

    /// <summary>
    /// Private constructor; use Make_ methods instead
    /// </summary>
    private PartyMember(double maxHP)
    {
      Max = maxHP;
      Current = Max;

      Height = HEALTH_BAR_HEIGHT;
      Width = HEALTH_BAR_WIDTH;

      ShowValues = true;
    }

    /// <summary>
    /// Restore HP to full, remove buff and debuff icons
    /// </summary>
    public void ResetStatus()
    {
      Current = Max;
      StatusBuff = "";
      StatusDebuff = "";
    }

    public const string WARRIOR_NAME = "Warrax";
    public const int WARRIOR_HP = 160000;
    /// <summary>
    /// Creates a protection warrior character
    /// </summary>
    /// <returns>Party Member representing a protection warrior</returns>
    public static PartyMember MakeProtectionWarrior()
    {
      return new PartyMember(WARRIOR_HP) { ColorBrush = new SolidColorBrush(Color.FromRgb(205, 184, 135)), DisplayLabel = WARRIOR_NAME, Tank = true, Melee = true };
    }

    public const string ROGUE_NAME = "Timmo";
    public const int ROGUE_HP = 120000;
    /// <summary>
    /// Creates a rogue character
    /// </summary>
    /// <returns>Party Member representing a rogue</returns>
    public static PartyMember MakeRogue()
    {
      return new PartyMember(ROGUE_HP) { ColorBrush = new SolidColorBrush(Color.FromRgb(220, 220, 0)), DisplayLabel = ROGUE_NAME, Melee = true };
    }

    public const string PRIEST_NAME = "Kintara";
    public const int PRIEST_HP = 110000;
    /// <summary>
    /// Creates a shadow priest character
    /// </summary>
    /// <returns>Party Member representing a shadow priest</returns>
    public static PartyMember MakeShadowPriest()
    {
      return new PartyMember(PRIEST_HP) { ColorBrush = new SolidColorBrush(Color.FromRgb(190, 190, 190)), DisplayLabel = PRIEST_NAME, Shadow = true };
    }

    public const string MAGE_NAME = "Ozzati";
    public const int MAGE_HP = 100000;
    /// <summary>
    /// Creates a mage character
    /// </summary>
    /// <returns>Party Member representing a mage</returns>
    public static PartyMember MakeMage()
    {
      return new PartyMember(MAGE_HP) { ColorBrush = new SolidColorBrush(Color.FromRgb(70, 130, 180)), DisplayLabel = MAGE_NAME };
    }

    public const int PALADIN_HP = 110000;
    /// <summary>
    /// Creates the paladin player character
    /// </summary>
    /// <param name="playerName">Name of the player</param>
    /// <returns>Party Member representing the player paladin</returns>
    public static PartyMember MakePaladinPlayer(string playerName)
    {
      return new PartyMember(PALADIN_HP) { ColorBrush = new SolidColorBrush(Color.FromRgb(219, 112, 143)), DisplayLabel = playerName, PlayerCharacter = true };
    }

    public static bool CheckEqualIgnoreCurrentStatus(PartyMember p1, PartyMember p2)
    {
      return p1.Max == p2.Max && p1.Name == p2.Name && p1.PlayerCharacter == p2.PlayerCharacter 
        && p1.Shadow == p2.Shadow && p1.Melee == p2.Melee && p1.Tank == p2.Tank;
    }

  }
}
