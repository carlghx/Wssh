using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;

using Wssh.Abilities;
using Wssh.UI;
using Wssh.Entities;

namespace Wssh
{
  /// <summary>
  /// Interaction logic for SpellButton.xaml
  /// </summary>
  public partial class SpellButton : UserControl
  {
    public SpellButton()
    {
      InitializeComponent();
    }

    private void BindOOM(GameState game)
    {
      rectOOM.DataContext = game.OOMUI;
    }

    /// <summary>
    /// Sets this button to turn blue if player has no Holy Power
    /// </summary>
    public void BindOOMToHolyPower(GameState game)
    {
      BindOOM(game);

      Binding b = new Binding();
      b.Path = new PropertyPath(OOMIndicatorUI.PropertyStringHolyPower);
      rectOOM.SetBinding(Rectangle.OpacityProperty, b);
    }

    /// <summary>
    /// Sets this button to turn blue if player does not have enough mana
    /// </summary>
    public void BindOOMToSpell(GameState game)
    {
      BindOOM(game);

      Binding b = new Binding();

      // get the spell being used
      PlayerAbility ability = (PlayerAbility)this.DataContext;
      switch (ability.Name)
      {
        case AbilityHolyLight.NAME:
          b.Path = new PropertyPath(OOMIndicatorUI.PropertyStringHolyLight);
          break;
        case AbilityDivineLight.NAME:
          b.Path = new PropertyPath(OOMIndicatorUI.PropertyStringDivineLight);
          break;
        case AbilityFlashOfLight.NAME:
          b.Path = new PropertyPath(OOMIndicatorUI.PropertyStringFlashOfLight);
          break;
        case AbilityHolyShock.NAME:
          b.Path = new PropertyPath(OOMIndicatorUI.PropertyStringHolyShock);
          break;
        case AbilityJudgment.NAME:
          b.Path = new PropertyPath(OOMIndicatorUI.PropertyStringJudgment);
          break;
        case AbilityAvengingWrath.NAME:
          b.Path = new PropertyPath(OOMIndicatorUI.PropertyStringAvengingWrath);
          break;
      }

      rectOOM.SetBinding(Rectangle.OpacityProperty, b);
    }
  }
}
