using System;

using Wssh.UI;
using Wssh.Abilities;
using Wssh.Abilities.EnemyAbilities;
using Wssh.Buffs;
using Wssh.Entities;

namespace Wssh.Utilities
{
  public class EnemyCastHandler
  {
    private GameState _game;

    public EnemyCastHandler(GameState game)
    {
      this._game = game;
    }

    /// <summary>
    /// Update all enemy abilities with new time and use any that are now ready
    /// </summary>
    /// <param name="timerIncSeconds">Amount of time since last tick, in seconds</param>
    public void UpdateTimer_Tick(double timerIncSeconds)
    {
      foreach (EnemyAbility ability in _game.ActiveEnemy.Abilities)
      {
        TickAbility(ability, timerIncSeconds);
      }
    }

    /// <summary>
    /// Update a specific enemy ability only. If updating all abilities, use UpdateTimer_Tick instead
    /// </summary>
    /// <param name="ability">Ability to update</param>
    /// <param name="timerIncSeconds">Amount of time since last tick, in seconds</param>
    public void TickAbility(EnemyAbility ability, double timerIncSeconds)
    {
      ability.CooldownRemaining -= timerIncSeconds;
      if (ability.CooldownRemaining <= 0)
      {
        ability.ExecuteCast(_game.ActiveParty.PartyMembers);
      }
    }

  }
}
