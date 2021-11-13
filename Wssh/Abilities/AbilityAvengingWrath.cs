using System;
using System.Collections.Generic;

using Wssh.Buffs;
using Wssh.Entities;
using Wssh.Utilities;

namespace Wssh.Abilities
{
  /// <summary>
  /// Increases all healing done for a short time
  /// </summary>
  public class AbilityAvengingWrath : PlayerAbility
  {
    public const string DEFAULT_KEY = "V";
    public HotkeyInfo HOTKEY = HotkeyInfo.CreateFromKeyString(DEFAULT_KEY);
    public const string NAME = "Avenging Wrath";
    public static int COOLDOWN = 180;
    public const int COST = 1851;

    public AbilityAvengingWrath(GameState game) : base(game)
    {
      _game = game;

      CastTime = 0;
      ManaCost = COST;
      Cooldown = COOLDOWN;
      UsesGCD = false;
      Hotkey = HotkeyInfo.CreateFromKeyString(DEFAULT_KEY);
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/AvengingWrath.png";
      Hotkey = HOTKEY;
      Name = NAME;
      Description = String.Format("{0}\n{1} mana\nIncreases all healing done by {2}% for {3} seconds.", Name, ManaCost, (BuffAvengingWrath.MODIFIER - 1) * 100, BuffAvengingWrath.DURATION);
      TargetType = AbilityTargetType.Single;
    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      if (ManaCost <= _game.ActivePlayer.ManaCurrent)
      {
        _game.ActivePlayer.ManaCurrent -= ManaCost;
        Buff newBuff = new BuffAvengingWrath(_game.ActivePlayer.PartyFrameUI);
        _game.ActivePlayer.AddOrRefreshBuff(newBuff, _game);
        
        PutOnCooldown();
        LogNonHeal();

        _game.ActivePlayer.LastCast.Update(new List<PartyMember>(), IconPath, false);

        return true;
      }

      return false;
    }

  }
}
