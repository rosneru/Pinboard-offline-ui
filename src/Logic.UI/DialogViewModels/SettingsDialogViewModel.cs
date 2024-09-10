using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.UI.Model;
using MvvmDialogs;

namespace Logic.UI.DialogViewModels
{
  public class SettingsDialogViewModel : ObservableObject, IModalDialogViewModel
  {
    public bool? DialogResult => _dialogResult;

    public ICommand CmdApply { get; }
    public ICommand CmdCancel { get; }
    public ICommand CmdDownloadJSON { get; }

    public  string JSONFileURL { get; set; }

    public  bool AskBeforeAppExit { get; set; }

    public SettingsDialogViewModel(IDialogService dialogService,
                                   IAppSettings settings,
                                   string appSettingsPath)
    {
      JSONFileURL = settings.JSONFileURL;
      AskBeforeAppExit = settings.AskBeforeAppExit;

      CmdApply = new RelayCommand(() =>
      {
        settings.JSONFileURL = JSONFileURL;
        settings.AskBeforeAppExit = AskBeforeAppExit;

        _dialogResult = true;
        dialogService.Close(this);
      }, () => (AskBeforeAppExit != settings.AskBeforeAppExit) ||
                JSONFileURL != settings.JSONFileURL);

      CmdCancel = new RelayCommand(() =>
      {
        _dialogResult = false;
        dialogService.Close(this);
      }, () => true);
    }

    private bool? _dialogResult;
  }
}
