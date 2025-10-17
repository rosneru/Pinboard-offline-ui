using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.UI.Model;
using Logic.UI.Services;
using Logic.UI.ViewModels;
using MvvmDialogs;

namespace Logic.UI.DialogViewModels
{
  public partial class FilterByTagsDialogViewModel : ObservableObject, IModalDialogViewModel
  {
    [ObservableProperty] private bool? _dialogResult;
    [ObservableProperty] private IBookmarkService _bookmarkService;

    public FilterByTagsDialogViewModel(ISettingsService settingsService,
                                       IDialogService dialogService,
                                       IBookmarkService bookmarkService)
    {
      _settingsService = settingsService;
      _dialogService = dialogService;
      BookmarkService = bookmarkService;
    }


    public void AddTag(object tag)
    {
      if(tag is DisplayedTag displayedTag)
      {
        BookmarkService.ToggleFilterTag(displayedTag.Name);
      }
    }


    [RelayCommand]
    private void Close()
    {
      DialogResult = false;
      _dialogService.Close(this);
    }

    ISettingsService _settingsService;
    IDialogService _dialogService;

  }

}
