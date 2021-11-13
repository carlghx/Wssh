using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace Wssh
{
  /// <summary>
  /// Interaction logic for HotkeyBox.xaml
  /// </summary>
  public partial class HotkeyBox : UserControl
  {
    /// <summary>
    /// Settings window that contains this box
    /// </summary>
    public SettingsWindow ParentSettings { get; set; }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(HotkeyBox));
    /// <summary>
    /// Text displayed in the box
    /// </summary>
    public string Text
    {
      get
      {
        //return text.Text;
        return (string)GetValue(TextProperty);
      }
      set
      {
        SetValue(TextProperty, value);
        HotkeyText.Text = value;
      }
    }

    public HotkeyBox()
    {
      InitializeComponent();
    }


    private void root_MouseDown(object sender, MouseButtonEventArgs e)
    {
      root.Background = new SolidColorBrush(Color.FromRgb(200, 200, 200));
      ParentSettings.HotkeyFocused(this);
    }

    public void UnFocus()
    {
      root.Background = null;
    }

  }
}
