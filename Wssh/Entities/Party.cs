using System.Collections.Generic;
using System.ComponentModel;


namespace Wssh.Entities
{
  /// <summary>
  /// A group of characters
  /// </summary>
  public class Party : INotifyPropertyChanged
  {
    private List<PartyMember> _partyMembers;
    /// <summary>
    /// Characters in this party
    /// </summary>
    public List<PartyMember> PartyMembers
    {
      get
      {
        if (_partyMembers == null)
          _partyMembers = new List<PartyMember>();

        return _partyMembers;
      }
    }

    /// <summary>
    /// The party member that represents the player character
    /// </summary>
    public PartyMember You { get; set; }

    /// <summary>
    /// Private constructor; use MakeDefaultParty() instead
    /// </summary>
    private Party()
    {

    }

    /// <summary>
    /// Creates a new party using default members
    /// </summary>
    /// <returns>Party with the 5 default members - warrior, rogue, shadow priest, mage, and holy paladin</returns>
    public static Party MakeDefaultParty(Player player)
    {
      Party party = new Party();

      PartyMember warrior = PartyMember.MakeProtectionWarrior();
      party.PartyMembers.Add(warrior);

      PartyMember rogue = PartyMember.MakeRogue();
      party.PartyMembers.Add(rogue);

      PartyMember priest = PartyMember.MakeShadowPriest();
      party.PartyMembers.Add(priest);

      PartyMember mage = PartyMember.MakeMage();
      party.PartyMembers.Add(mage);

      PartyMember paladin = player.PartyFrameUI;
      party.You = paladin;
      party.PartyMembers.Add(paladin);

      return party;
    }

    /// <summary>
    /// Reset status of everyone in party
    /// </summary>
    public void ResetStatus()
    {
      foreach (PartyMember member in PartyMembers)
      {
        member.ResetStatus();
      }
    }


    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;
    public virtual void OnPropertyChanged(string s)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(s));
    }
    #endregion
  }
}
