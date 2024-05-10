using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.UI.DialogViewModels;
using Logic.UI.PageViewModels;
using Logic.UI.Tools;
using MvvmDialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Logic.UI.ViewModels
{
  public class MainViewModel : ObservableObject
  {
    public ICommand CmdOpen { get; }
    public ICommand CmdExit { get; }


    public List<PageViewModelBase> PageViewModels { get; set; }
    public PageViewModelBase CurrentPageViewModel { get; set; }

    public UITools UiTools { get; }

    public MainViewModel(IDialogService dialogService)
    {
      UiTools = new UITools(dialogService);

      // Create the PageViewModels
      var connectionViewModel = new ConnectionViewModel(UiTools);
      var firmwareViewModel = new FirmwareViewModel(UiTools);
      var settingsViewModel = new SettingsViewModel(UiTools);

      // And add them to the 'list of pages'
      PageViewModels = new List<PageViewModelBase>
      {
        connectionViewModel,
        firmwareViewModel,
        settingsViewModel,
      };

      // Select the first view model to be displayed as default
      PageViewModels[0].IsOpen = true;
      CurrentPageViewModel = PageViewModels[0];

      CmdOpen = new RelayCommand(() =>
      {
        var openDialog = new OpenDeviceDialogViewModel(UiTools.DialogService);
        var success = UiTools.DialogService.ShowDialog(this, openDialog);
        if (success == true)
        {
          // Open the device e.g. by opening openDialog.Id from database
          var msg = $"The device '{openDialog.SelectedDeviceFile.FileName}' will now be loaded";
          UiTools.DialogService.ShowMessageBox(this,
                                               msg,
                                               "Loading file",
                                               MessageBoxButton.OK,
                                               MessageBoxImage.Information);
        }
      }, () => true);

      CmdExit = new RelayCommand<object>((p) =>
      {
        if (isShutdownCommitted)
        {
          // The Application.Current.Shutdown() below leads to a 2nd
          // invocation of this command. Can be skipped.
          return;
        }

        if (p != null)
        {
          var cancelEventArgs = p as CancelEventArgs;
          if (cancelEventArgs != null)
          {
            // Ok, the command was probably triggered by a WindowCLose
            // event. We cancel that WindowsClose because shutdown is
            // done manually depending on the MessageBox result below.
            cancelEventArgs.Cancel = true;
          }
        }

        var result = UiTools.DialogService.ShowMessageBox(this,
                                                          "Do you really want to quit the application?",
                                                          "Really exit?",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
          isShutdownCommitted = true;
          Application.Current.Shutdown();
        }
      }, (p) => true);
    }

    bool isShutdownCommitted = false;
  }
}
