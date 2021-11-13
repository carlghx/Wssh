
using Wssh.Entities;

namespace Wssh.Utilities
{
  /// <summary>
  /// This class tracks whether ctrl or shift are currently down, and determines whether a hotkey combination has been pressed
  /// (alt is not tracked because it is already used by the window menu)
  /// </summary>
  public class KeyModHandler
  {

    public const string LEFT_SHIFT = "LeftShift";
    public const string RIGHT_SHIFT = "RightShift";
    public const string LEFT_CTRL = "LeftCtrl";
    public const string RIGHT_CTRL = "RightCtrl";

    public const string TAB = "Tab";

    /// <summary>
    /// Whether the Ctrl key is down
    /// </summary>
    public bool CtrlDown { get; set; }

    /// <summary>
    /// Whether the Shift key is down
    /// </summary>
    public bool ShiftDown { get; set; }


    public KeyModHandler()
    {
      CtrlDown = false;
      ShiftDown = false;
    }

    /// <summary>
    /// Checks to see if the input string matches the given hotkey info, including modifier keys
    /// </summary>
    /// <param name="inputStr">Keycode + any modifiers (e.g. shift-ctrl-D4)</param>
    /// <param name="info">Hotkey to match</param>
    /// <returns>True if input matches hotkey; false otherwise</returns>
    public bool MatchHotkey(string inputStr, HotkeyInfo info)
    {
      string formattedInputStr = HotkeyInfo.GetDisplayString(inputStr, ShiftDown, CtrlDown);
      string formattedKeyStr = HotkeyInfo.GetDisplayString(info.KeyString, ShiftDown, CtrlDown);
      if (formattedKeyStr == formattedInputStr
          && info.CtrlDown == CtrlDown && info.ShiftDown == ShiftDown)
      {
        return true;
      }

      return false;
    }

    public static bool InvalidKeyStr(string keyStr)
    {
      return (keyStr == "Return" || keyStr == "LWin" || keyStr == "RWin" || keyStr == "System" || keyStr == "LeftAlt" || keyStr == "RightAlt");
    }

    public static bool ArrowKeyStr(string keyStr)
    {
      return (keyStr == "Up" || keyStr == "Down" || keyStr == "Left" || keyStr == "Right");
    }

  }
}
