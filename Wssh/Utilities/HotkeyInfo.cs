using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Wssh.Utilities
{
  /// <summary>
  /// Contains a key and optional modifiers (shift and ctrl)
  /// </summary>
  public class HotkeyInfo : INotifyPropertyChanged
  {

    /// <summary>
    /// Whether this hotkey requires Shift to be pressed
    /// </summary>
    public bool ShiftDown { get; set; }

    /// <summary>
    /// Whether this hotkey requires Ctrl to be pressed
    /// </summary>
    public bool CtrlDown { get; set; }

    /// <summary>
    /// The C# keycode used by this hotkey (e.g. D4 for 4 or OemQuotes for ')
    /// </summary>
    public string KeyString { get; set; }

    public static string PropertyStringDisplay = MemberNameFinder<HotkeyInfo>.GetMemberName(x => x.DisplayString);
    private string _displayString;
    /// <summary>
    /// the user-friendly string used to show this hotkey
    /// </summary>
    public string DisplayString 
    {
      get
      {
        return _displayString;
      }
      set
      {
        _displayString = value;
        OnPropertyChanged(PropertyStringDisplay);
      }
    }

    public void UpdateFromInputString(string keyStr)
    {
      string newKeyString = "";
      bool ctrl = false, shift = false;

      string[] inputStrings = keyStr.Split('-');
      newKeyString = inputStrings[inputStrings.Length - 1];
      for (int i = 0; i < inputStrings.Length - 1; i++)
      {
        string input = inputStrings[i].ToUpper();
        switch (input)
        {
          case "CTRL":
            ctrl = true;
            break;
          case "SHIFT":
            shift = true;
            break;
        }
      }

      SetHotkeyInfo(newKeyString, shift, ctrl);
    }

    /// <summary>
    /// Parses an input string of the form ctrl-shift-key and returns a hotkey object 
    /// </summary>
    /// <param name="keyStr">String to be parsed</param>
    /// <returns>Hotkey object describing the input string</returns>
    public static HotkeyInfo CreateFromKeyString(string keyStr)
    {
      HotkeyInfo newInfo = new HotkeyInfo();
      newInfo.UpdateFromInputString(keyStr);

      return newInfo;
    }

    /// <summary>
    /// Private constructor; use CreateFromKeyString instead
    /// </summary>
    private HotkeyInfo()
    {

    }

    private void SetHotkeyInfo(string keyString, bool shift, bool ctrl)
    {
      ShiftDown = shift;
      CtrlDown = ctrl;
      KeyString = keyString;

      DisplayString = GetDisplayString(KeyString, ShiftDown, CtrlDown);
    }

    /// <summary>
    /// Converts C# keycodes into user-friendly names
    /// </summary>
    /// <param name="inputString">C# keycode to convert</param>
    /// <param name="shift">Whether this hotkey requires shift</param>
    /// <param name="ctrl">Whether this hotkey requires ctrl</param>
    /// <returns>User-friendly display name for this hotkey</returns>
    public static string GetDisplayString(string inputString, bool shift, bool ctrl)
    {
      string returnString = inputString;
      Regex expression = new Regex("^D[0-9]$");
      if (expression.IsMatch(inputString))
      {
        returnString = inputString[1].ToString();
      }

      switch (inputString)
      {
        case "Capital":
          returnString = "Caps";
          break;

        case "System":
          returnString = "Alt";
          break;

        case "OemOpenBrackets":
          returnString = "[";
          break;

        case "OemQuotes":
          returnString = "'";
          break;

        case "OemPeriod":
          returnString = ".";
          break;

        case "OemComma":
          returnString = ",";
          break;

        case "OemQuestion":
          returnString = "?";
          break;

        case "Oem1":
          returnString = ";";
          break;

        case "Oem2":
          returnString = "/";
          break;

        case "Oem3":
          returnString = "`";
          break;

        case "Oem5":
          returnString = @"\";
          break;

        case "Oem6":
          returnString = "]";
          break;

      }

      if (ctrl)
        returnString = "ctrl-" + returnString;
      if (shift)
        returnString = "shift-" + returnString;

      return returnString;
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;
    
    public void OnPropertyChanged(string propertyName)
    {
      if(PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }

}
