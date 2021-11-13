using System.Collections.Generic;
using System.ComponentModel;

using Wssh.Entities;


namespace Wssh.Abilities
{
  /// <summary>
  /// Contains an Ability and the Ability's targets
  /// </summary>
  public class Cast : INotifyPropertyChanged
  {
    /// <summary>
    /// The ability that this cast is using
    /// </summary>
    public Ability Ability { get; set; }

    private List<PartyMember> _targets;
    /// <summary>
    /// All targets for this cast
    /// </summary>
    public List<PartyMember> Targets
    {
      get
      {
        if (_targets == null)
          _targets = new List<PartyMember>();

        return _targets;
      }
      set
      {
        _targets = value;
      }
    }

    /// <summary>
    /// Executes a cast on all of its specified targets
    /// </summary>
    /// <returns>True if cast is successful; false otherwise</returns>
    public bool ExecuteCast()
    {
      return Ability.ExecuteCast(Targets);
    }


    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
      OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
    {
      var handler = PropertyChanged;
      if (handler != null)
        handler(this, args);
    }
    #endregion
  }
}
