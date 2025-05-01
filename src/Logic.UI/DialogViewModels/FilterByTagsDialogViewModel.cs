using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.UI.Services;
using Logic.UI.ViewModels;
using MvvmDialogs;

namespace Logic.UI.DialogViewModels
{
  public partial class FilterByTagsDialogViewModel : ObservableObject, IModalDialogViewModel
  {
    [ObservableProperty] private bool? _dialogResult;
    [ObservableProperty] private IBookmarkService _bookmarkService;

    [ObservableProperty] private string _tagToAdd;

    public FilterByTagsDialogViewModel(ISettingsService settingsService,
                                       IDialogService dialogService,
                                       IBookmarkService bookmarkService)
    {
      _settingsService = settingsService;
      _dialogService = dialogService;
      BookmarkService = bookmarkService;
    }

    private bool CanExecuteAddTag()
    {
      return !string.IsNullOrEmpty(TagToAdd);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteAddTag))]
    private void AddTag()
    {
      if (BookmarkService.FilteredTags.Any(
        tag => tag.Equals(TagToAdd, StringComparison.OrdinalIgnoreCase)))
      {
        _dialogService
          .ShowMessageBox(this,
                          $"The tag '{TagToAdd}' is already filtered",
                          "Tag already added",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        return;
      }

      if (!BookmarkService.AllBookmarks.Any(
        bm => bm.TagsArray.Any(tag => tag.Equals(
          TagToAdd, StringComparison.OrdinalIgnoreCase))))
      {
        _dialogService
          .ShowMessageBox(this,
                          $"The tag '{TagToAdd}' not found in any bookmark.",
                          "Tag not found",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        return;
      }

      BookmarkService.ToggleFilterTag(TagToAdd);
      TagToAdd = "";
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
