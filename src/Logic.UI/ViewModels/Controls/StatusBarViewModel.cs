using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Logic.UI.ViewModels.Controls
{
  /// <summary>
  /// A status bar with the ability to set the displayed status
  /// text, the logged on user and the application version.
  ///
  /// Also it has a public <see cref="AddNotification(string)"/> method.
  /// All text messages notified this way are written one after another
  /// into the <see cref="Notification"/> property and stay there for a
  /// short period of time (constant <see
  /// cref="notificationDuration_ms"/>). The view should bind to this
  /// property and display its contents maybe flashing to ensure that
  /// the user will notice it.
  /// </summary>
  public class StatusBarViewModel : ObservableObject
  {
    private const int notificationDuration_ms = 550;

    /// <summary>
    /// Holds the last notification message for a distinct time span,
    /// then is set to empty.
    /// </summary>
    public string Notification { get; private set; }

    /// <summary>
    /// The text about current application state to be displayed in the
    /// status bar.
    /// </summary>
    public string StatusText { get; set; } = "Ready";

    /// <summary>
    /// The current user name or login name  to be displayed in the
    /// status bar.
    /// </summary>
    public string LoginName { get; set; } = "-";

    /// <summary>
    /// The current application version  to be displayed in the status
    /// bar.
    /// </summary>
    public string AppVersion { get; set; } = "-";

    SynchronizationContext uiTaskContext = SynchronizationContext.Current;

    public StatusBarViewModel(CancellationToken token)
    {
      notificationsQueue = new Queue<string>();

      Task.Run(() =>
      {
        // Exit the task by exception when cancellation is requested
        token.ThrowIfCancellationRequested();
        try
        {
          do
          {
            if (notificationsQueue.Count > 0)
            {
              uiTaskContext.Post(state =>
              {
                Notification = notificationsQueue.Dequeue();
              }, null);

              // Wait some time to hold the error displayed
              token.WaitHandle.WaitOne(notificationDuration_ms);
            }
            else
            {
              uiTaskContext.Post(state =>
              {
                Notification = "";
              }, null);
            }

            // Wait some milliseconds between the queue polls
            token.WaitHandle.WaitOne(300);
          }
          while (token.IsCancellationRequested == false);
        }
        catch(OperationCanceledException)
        {
          // No special handling of 'cancel' needed here, just exit the
          // task
        }
        catch(Exception e)
        {
          // The notification loop broke because of an exception:
          // set the exception message as last visible Notification
          Notification = $"{nameof(StatusBarViewModel)}.cs: {e.Message}";
        }
      }, token);
    }


    /// <summary>
    /// Adds an notification to be displayed in the <see
    /// cref="Notification"/> property as soon as the previously added
    /// errors have been displayed.
    /// </summary>
    /// <param name="errorText"></param>
    public void AddNotification(string errorText)
    {
      if (string.IsNullOrEmpty(errorText))
      {
        return;
      }

      notificationsQueue.Enqueue(new string(errorText));
    }


    Queue<string> notificationsQueue;
  }
}
