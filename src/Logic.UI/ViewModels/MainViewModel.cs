using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Config.Net;
using Logic.UI.DialogViewModels;
using Logic.UI.Model;
using Logic.UI.PageViewModels;
using Logic.UI.Tools;
using MvvmDialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Logic.UI.ViewModels
{
  public class MainViewModel : ObservableObject
  {
    public ICommand CmdSettings { get; }
    public ICommand CmdUpdate { get; }
    public ICommand CmdExit { get; }


    public List<PageViewModelBase> PageViewModels { get; set; }
    public PageViewModelBase CurrentPageViewModel { get; set; }

    public UITools UiTools { get; }

    public MainViewModel(IDialogService dialogService)
    {
      var appDataRoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      var developerName = "tysw";
      var appName = "Pinboard-offline-ui";

      var appSettingsPath = Path.Combine(appDataRoamingPath, developerName, appName);

      if (!Directory.Exists(appSettingsPath))
      {
        Directory.CreateDirectory(appSettingsPath);
      }

      var appSettingsFile = Path.Combine(appSettingsPath, "settings.json");
      IAppSettings settings = new ConfigurationBuilder<IAppSettings>()
        .UseJsonFile(appSettingsFile)
        .Build();

      UiTools = new UITools(dialogService);

      // Create the PageViewModels
      var connectionViewModel = new ConnectionViewModel(UiTools);
      var firmwareViewModel = new FirmwareViewModel(UiTools);

      // And add them to the 'list of pages'
      PageViewModels = new List<PageViewModelBase>
      {
        connectionViewModel,
        firmwareViewModel,
      };

      // Select the first view model to be displayed as default
      PageViewModels[0].IsOpen = true;
      CurrentPageViewModel = PageViewModels[0];

      CmdSettings = new RelayCommand(() =>
      {
        var openDialog = new SettingsDialogViewModel(UiTools.DialogService,
                                                     settings,
                                                     appSettingsPath);
        var success = UiTools.DialogService.ShowDialog(this, openDialog);
        if (success == true)
        {
          // Open the device e.g. by opening openDialog.Id from database
          // TODO Load content from JSON
        }
      }, () => true);

      CmdUpdate = new RelayCommand(() =>
      {
        var settingsDialog = new UpdateDialogViewModel(UiTools.DialogService,
                                                     settings,
                                                     appSettingsPath);
        var success = UiTools.DialogService.ShowDialog(this, settingsDialog);
        if (success == true)
        {
          // Open the device e.g. by opening openDialog.Id from database
          // TODO Load content from JSON
        }
      }, () => true);

      CmdExit = new RelayCommand<object>((p) =>
      {
        if (_isExitAccepted)
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

        MessageBoxResult result = MessageBoxResult.Yes;
        if(settings.AskBeforeAppExit)
        {
          result = UiTools
                     .DialogService
                     .ShowMessageBox(this,
                                     "Do you really want to quit the application?",
                                     "Really exit?",
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Warning);
        }

        if (result == MessageBoxResult.Yes)
        {
          _isExitAccepted = true;
          Application.Current.Shutdown();
        }
      }, (p) => true);
    }



    bool _isExitAccepted = false;
  }
}
