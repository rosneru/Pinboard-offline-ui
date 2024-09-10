using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MvvmDialogs;

namespace Logic.UI.DialogViewModels
{
  public class UpdateDialogViewModel : ObservableObject, IModalDialogViewModel
  {
    public bool? DialogResult => throw new NotImplementedException();
  }
}
