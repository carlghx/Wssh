using System;
using System.Collections.Generic;

using Wssh.Entities;
using Wssh.Utilities;
using Wssh.Buffs;

namespace Wssh.Abilities
{
  public class AbilityJudgment : PlayerAbility
  {
    public const string DEFAULT_KEY = "A";
    public const string NAME = "Judgment";
    public static int COOLDOWN = 8;
    public const int COST = 1157;

    public AbilityJudgment(GameState game) : base(game)
    {
      CastTime = 0;
      ManaCost = COST;
      Cooldown = COOLDOWN;
      UsesGCD = true;
      Hotkey = HotkeyInfo.CreateFromKeyString(DEFAULT_KEY);
      IconPath = "pack://application:,,/Wssh;component/Images/Icons/Judgment.png";
      Name = NAME;
      Description = String.Format("{0}\n{1} mana\nRestores mana and increases casting speed for 1 minute.", Name, ManaCost);
      TargetType = AbilityTargetType.Single;
    }

    public override bool ExecuteCast(List<PartyMember> targets)
    {
      if (ManaCost <= _game.ActivePlayer.ManaCurrent)
      {
        _game.ActivePlayer.ManaCurrent -= ManaCost;

        Buff newBuff = new BuffJudgment(_game.ActivePlayer.PartyFrameUI);
        _game.ActivePlayer.AddOrRefreshBuff(newBuff, _game);
        _game.ActivePlayer.ManaCurrent += COST * 3;
        _game.ActivePlayer.LastCast.Update(new List<PartyMember>(), IconPath, false);

        PutOnCooldown();

        Random random = App.RandomGen;
        bool crit;
        crit = (random.NextDouble() <= _game.ActivePlayer.CritChance);

        LogNonHeal(crit);

        return true;
      }
      
      return false;
    }

  }
}
