using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Wssh
{
  /// <summary>
  /// Interaction logic for NumericUpDown.xaml
  /// </summary>
  public partial class NumericUpDown : UserControl
  {
    /// <summary>
    /// Minimum value
    /// </summary>
    public int Min { get; set; }

    /// <summary>
    /// Maximum value
    /// </summary>
    public int Max { get; set; }

    /// <summary>
    /// Amount to change value by after an arrow up/down
    /// </summary>
    public int Increment { get; set; }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(NumericUpDown));
    /// <summary>
    /// Current displayed value
    /// </summary>
    public int Value
    {
      get
      {
        return (int)GetValue(ValueProperty);
      }
      set
      {
        int newValue = value;
        if (newValue < Min)
          newValue = Min;
        if (newValue > Max)
          newValue = Max;

        SetValue(ValueProperty, newValue);
        box.Text = newValue.ToString();
      }
    }

    public NumericUpDown()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Increase current value by Increment
    /// </summary>
    private void DoIncrement()
    {
      Value += Increment;
    }

    /// <summary>
    /// Decrease current value by Increment
    /// </summary>
    private void DoDecrement()
    {
      Value -= Increment;
    }

    private void box_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (e.Delta > 0)
        DoIncrement();
      else
        DoDecrement();
    }

    private void btnUp_Click(object sender, RoutedEventArgs e)
    {
      DoIncrement();
    }

    private void btnDown_Click(object sender, RoutedEventArgs e)
    {
      DoDecrement();
    }

    private void box_LostFocus(object sender, RoutedEventArgs e)
    {
      int oldVal = Value;
      try
      {
        int newVal = int.Parse(box.Text);
        Value = newVal;
        box.Text = Value.ToString();
      }
      catch
      {
        box.Text = oldVal.ToString();
      }
    }

  }
}
