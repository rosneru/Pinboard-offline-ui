using CommunityToolkit.Mvvm.Input;
using Logic.UI.Tools;
using Logic.UI.ViewModels;
using PropertyChanged;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Logic.UI.PageViewModels
{
  public class ConnectionViewModel : PageViewModelBase
  {

    public ICommand CmdPing { get; }

    public ICommand CmdConnect { get; }

    public string IPAddress { get; set; } = "192.168.1.1";


    public ConnectionViewModel(UITools UiTools)
      : base("Connection", "\uE8A1", UiTools)
    {

      CmdPing = new RelayCommand(() =>
      {

      }, () => !string.IsNullOrEmpty(IPAddress));

      CmdConnect = new RelayCommand(() => 
      {
      
      }, () => !string.IsNullOrEmpty(IPAddress));

    }

  }
}
