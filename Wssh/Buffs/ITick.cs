using Wssh.Entities;

namespace Wssh.Buffs
{
  /// <summary>
  /// Indicates a generic tick buff or debuff
  /// </summary>
  interface ITick
  {
    bool CheckTick();
    double Timer { get; set; }
  }

  /// <summary>
  /// Indicates a damage over time debuff
  /// </summary>
  interface IDoT : ITick
  {
    int GetDamage(GameState game);
  }

  /// <summary>
  /// Indicates a heal over time buff
  /// </summary>
  interface IHoT : ITick
  {
    int GetHeal();
  }
}
