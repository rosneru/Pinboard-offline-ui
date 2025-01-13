using System.Threading;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.UI.ViewModels.Controls;
using MvvmDialogs;

namespace Logic.UI.Tools
{
  /// <summary>
  /// Contains commands, properties, a message box, the status bar and
  /// some more useful tools. Is needed in most view models, so it
  /// should be created once and passed to all view models per
  /// reference.
  ///
  /// It also holds the menu background color which at the moment is
  /// fixed set in a private variable. TODO This could be made a global
  /// setting and loaded from database.
  /// </summary>
  public partial class UITools : ObservableObject
  {

    [ObservableProperty] private ICommand _cmdMenuLock;
    [ObservableProperty] private ICommand _cmdMenuUnlock;
    [ObservableProperty] private ICommand _cmdCancel;

    [ObservableProperty] private IDialogService _dialogService;

    [ObservableProperty] private bool _isMenuLocked;

    [ObservableProperty] private StatusBarViewModel _statusBar;

    [ObservableProperty] private CancellationTokenSource _cancelToken;

    public UITools(IDialogService dialogService)
    {
      DialogService = dialogService;
      ctsQuit = new CancellationTokenSource();

      CmdMenuUnlock = new RelayCommand(() =>
      {
        IsMenuLocked = false;
      });

      CmdMenuLock = new RelayCommand(() =>
      {
        IsMenuLocked = true;
      });

      CmdCancel = new RelayCommand(() =>
      {
        CancelToken?.Cancel();
      });


      // The status bar has a permanent running task for the
      // notification queue which can be quit using the given token.
      StatusBar = new StatusBarViewModel(ctsQuit.Token);
    }

    public void QuitAllTasks()
    {
      CancelToken?.Cancel();
      ctsQuit?.Cancel();
    }

    private CancellationTokenSource ctsQuit;
  }
}
