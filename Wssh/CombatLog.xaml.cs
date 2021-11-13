using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

using Wssh.UI;

namespace Wssh
{
  /// <summary>
  /// Interaction logic for CombatLog.xaml
  /// </summary>
  public partial class CombatLog : UserControl
  {
    /// <summary>
    /// Console info used when logging messages
    /// </summary>
    public ConsoleUI Console { get; set; }

    private ObservableCollection<MessageUI> _messages;
    /// <summary>
    /// Messages stored in this log
    /// </summary>
    public ObservableCollection<MessageUI> MessageLog
    {
      get
      {
        if (_messages == null)
          _messages = new ObservableCollection<MessageUI>();

        return _messages;
      }
    }

    public CombatLog()
    {
      InitializeComponent();

      listMessages.ItemsSource = MessageLog;
    }

    /// <summary>
    /// Add a new message to the combat log
    /// </summary>
    /// <param name="message">Message to add</param>
    public void LogMessage(string message)
    {
      if (Console == null)
        return;

      TimeSpan ts = new TimeSpan(0, 0, (int)Console.ElapsedTime);
      MessageUI ui = new MessageUI() { Message = message, TimeCode = ts.ToString() };
      MessageLog.Add(ui);

      scroll.ScrollToBottom();
    }

  }
}
