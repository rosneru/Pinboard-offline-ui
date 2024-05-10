using CommunityToolkit.Mvvm.ComponentModel;
using Logic.UI.Tools;
using Logic.UI.ViewModels;


namespace Logic.UI.PageViewModels
{
  public abstract class PageViewModelBase : ObservableObject
  {
    public string Name { get; }
    public string Symbol { get; }

    public string ErrorText { get; set; }


    public string SuccessText { get; set; }

    /// <summary>
    /// True if this ViewModel is the one currently open.
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// True if this ViewModel is locked until either apply or reject 
    /// action is done.
    /// </summary>
    public bool IsLocked { get; set; }

    public UITools UiTools { get; }

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
