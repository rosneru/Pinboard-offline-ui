using System;
using System.Collections.Generic;
using System.Linq;
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
    public bool? DialogResult => dialogResult;

    public ICommand CmdApply { get; }
    public ICommand CmdCancel { get; }
    public ICommand CmdDownloadJSON { get; }

    public  string JSONFileURL { get; set; }
    public  bool AskBeforeAppExit { get; set; }

    public SettingsDialogViewModel(IDialogService dialogService,
                                   IAppSettings settings)
    {
      JSONFileURL = settings.JSONFileURL;
      AskBeforeAppExit = settings.AskBeforeAppExit;

      CmdApply = new RelayCommand(() =>
      {
        settings.AskBeforeAppExit = AskBeforeAppExit;

        dialogResult = true;
        dialogService.Close(this);
      }, () => AskBeforeAppExit != settings.AskBeforeAppExit);

      CmdCancel = new RelayCommand(() =>
      {
        dialogResult = false;
        dialogService.Close(this);
      }, () => true);

      CmdDownloadJSON = new RelayCommand(() =>
      {
        // TODO: Download the JSON file and only if it succeeds, do:
        settings.JSONFileURL = JSONFileURL;
      }, () =>JSONFileURL != settings.JSONFileURL);
    }

    private bool? dialogResult;
  }
}
