using System.Collections.Generic;
using System.ComponentModel;

using Wssh.Entities;
using Wssh.Utilities;

namespace Wssh.UI
{
  /// <summary>
  /// UI for the last cast ItemsControl in MainWindow
  /// </summary>
  public class LastCastUI : INotifyPropertyChanged
  {
    /// <summary>
    /// Opacity % of enemy cast list when it is covered by combat log
    /// </summary>
    public const double OPACITY_FADE = 0.05;

    public string PropertyStringIconPath = MemberNameFinder<LastCastUI>.GetMemberName(x => x.IconPath);
    private string _iconPath;
    /// <summary>
    /// Path containing the icon of the spell used for this cast
    /// </summary>
    public string IconPath
    {
      get
      {
        return _iconPath;
      }
      set
      {
        _iconPath = value;
        OnPropertyChanged(PropertyStringIconPath);
      }
    }

    public string PropertyStringTarget = MemberNameFinder<LastCastUI>.GetMemberName(x => x.Target);
    private string _target;
    /// <summary>
    /// Target of the last spell cast
    /// </summary>
    public string Target
    {
      get
      {
        return _target;
      }
      set
      {
        _target = value;
        OnPropertyChanged(PropertyStringTarget);
      }
    }

    public string PropertyStringMessage = MemberNameFinder<LastCastUI>.GetMemberName(x => x.Message);
    private string _message;
    /// <summary>
    /// Additional information about the cast
    /// </summary>
    public string Message
    {
      get
      {
        return _message;
      }
      set
      {
        _message = value;
        OnPropertyChanged(PropertyStringMessage);
      }
    }

    /// <summary>
    /// Update the currently displayed cast
    /// </summary>
    /// <param name="targets">List of party members affected by this cast</param>
    /// <param name="icon">Icon path for the new ability</param>
    /// <param name="crit">Whether the ability was a critical hit</param>
    public void Update(List<PartyMember> targets, string icon, bool crit = false)
    {
      Target = "";
      if (targets != null && targets.Count == 1)
        Target = targets[0].Name;
      IconPath = icon;
      if (crit)
        Message = "Crit";
      else
        Message = "";
    }

    /// <summary>
    /// Remove current spell information so that last cast displays nothing
    /// </summary>
    public void Clear()
    {
      Message = "";
      Target = "";
      IconPath = "";
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
