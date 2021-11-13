using System.Collections.Generic;

using Wssh.Entities;
using Wssh.Buffs;
using System;
using Wssh.Utilities;

namespace Wssh.Abilities
{
  public class AbilityDivinePlea : PlayerAbility
  {
    public const string DEFAULT_KEY = "7";
    public const string NAME = "Divine Plea";
    public static int COOLDOWN = 120;
    public const int COST = 0;    

    public AbilityDivinePlea(GameState game) : base(game)
    {
      CastTime = 0;
      ManaCost = COST;
      Cooldown = COOLDOWN;
      UsesGCD = true;
      Hotkey = HotkeyInfo.CreateFromKeyString(DEFAULT_KEY);
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/DivinePlea.png";
      Name = NAME;
      Description = String.Format("{0}\nRestores 18% of total mana over 9 seconds, but reduces healing done by {1}%.", Name, BuffDivinePlea.HEAL_MODIFIER * 100);
      TargetType = AbilityTargetType.Single;
    }


    public override bool ExecuteCast(List<PartyMember> targets)
    {
      if (ManaCost <= _game.ActivePlayer.ManaCurrent)
      {
        _game.ActivePlayer.ManaCurrent -= ManaCost;
        Buff newBuff = new BuffDivinePlea(_game.ActivePlayer.PartyFrameUI);
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
