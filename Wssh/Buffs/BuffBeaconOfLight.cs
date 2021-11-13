
using Wssh.Entities;
using Wssh.Utilities;
using System;

namespace Wssh.Buffs
{
  /// <summary>
  /// Heals directed at other targets will also heal the Beacon of Light
  /// </summary>
  public class BuffBeaconOfLight : Buff
  {
    public const string NAME = "Beacon Of Light";
    public static int DURATION = 300;

    public BuffBeaconOfLight(PartyMember target)
    {

      IconPath = "pack://application:,,/Wssh;component/Images/Icons/BeaconOfLight.png";
      DurationRemaining = DURATION;
      Target = target;
      Name = NAME;
      Description = String.Format("{0}\nHeals directed at other targets will also heal this target.", Name);
    }
  }
}
