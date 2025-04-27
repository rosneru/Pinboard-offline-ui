using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Logic.UI.ViewModels.Controls;

namespace Logic.UI.Services
{
  public interface IUiService
  {
    bool IsMenuLocked { get; set; }

    CancellationTokenSource CancelToken { get; set; }

    void QuitAllTasks();
  }
}
