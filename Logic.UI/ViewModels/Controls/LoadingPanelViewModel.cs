using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Logic.UI.ViewModels.Controls
{
  public class LoadingPanelViewModel : ObservableObject
  {
    public bool IsBusy { get; set; }

    public string BusyMessage { get; set; } = "Loading...";

    public bool IsBusyProgressVisible { get; set; } = false;

    public double BusyProgressValue { get; set; } = 0.0f;
  }
}
