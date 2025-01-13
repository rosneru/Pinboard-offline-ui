using CommunityToolkit.Mvvm.ComponentModel;
using Logic.UI.Tools;
using Logic.UI.ViewModels;


namespace Logic.UI.PageViewModels
{
  public abstract partial class PageViewModelBase : ObservableObject
  {
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _symbol;

    [ObservableProperty] private string _errorText;


    [ObservableProperty] private string _successText;

    /// <summary>
    /// True if this ViewModel is the one currently open.
    /// </summary>
    [ObservableProperty] private bool _isOpen;

    /// <summary>
    /// True if this ViewModel is locked until either apply or reject 
    /// action is done.
    /// </summary>
    [ObservableProperty] private bool _isLocked;

    [ObservableProperty] private UITools _uiTools;

    public PageViewModelBase(string name,
                             string symbol,
                             UITools UiTools)
    {
      Name = name;
      Symbol = symbol;
      this.UiTools = UiTools;
    }
  }
}
