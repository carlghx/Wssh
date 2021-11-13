using System.Windows.Controls;
using System.ComponentModel;

using Wssh.UI;
using Wssh.Utilities;

namespace Wssh
{
  /// <summary>
  /// Interaction logic for DisplayBar.xaml
  /// </summary>
  public partial class DisplayBar : UserControl, INotifyPropertyChanged
  {
    public string PropertyStringUI = MemberNameFinder<DisplayBar>.GetMemberName(x => x.UI);
    private BarUI _ui;
    public BarUI UI
    {
      get
      {
        return _ui;
      }
      set
      {
        _ui = value;
        DataContext = value;
        OnPropertyChanged(PropertyStringUI);
      }
    }

    public DisplayBar()
    {
      InitializeComponent();
    }

    public void SetWidth(int width)
    {
      Width = width;
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
