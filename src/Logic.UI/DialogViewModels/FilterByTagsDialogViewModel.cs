using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.UI.Services;
using MvvmDialogs;

namespace Logic.UI.DialogViewModels
{
  public partial class FilterByTagsDialogViewModel : ObservableObject, IModalDialogViewModel
  {
    [ObservableProperty] private bool? _dialogResult;

    public FilterByTagsDialogViewModel(ISettingsService settingsService,
                                       IDialogService dialogService)
    {
      _settingsService = settingsService;
      _dialogService = dialogService;
    }

    private bool CanExecuteApply()
    {
      return false;
    }

    [RelayCommand(CanExecute = nameof(CanExecuteApply))]
    private void Apply()
    {

      DialogResult = true;
      _dialogService.Close(this);
    }

    [RelayCommand]
    private void Cancel()
    {
      DialogResult = false;
      _dialogService.Close(this);
    }

    ISettingsService _settingsService;
    IDialogService _dialogService;

  }
  
}
