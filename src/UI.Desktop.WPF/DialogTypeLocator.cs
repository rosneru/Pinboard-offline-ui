using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmDialogs.DialogTypeLocators;

namespace UI.Desktop.WPF
{
  /// <summary>
  /// Read more about the configuration of custom dialogs in the 
  /// 'MVVM dialogs' package documentation:
  /// https://github.com/FantasticFiasco/mvvm-dialogs/wiki/Custom-dialog-type-locators
  /// </summary>
  public class DialogTypeLocator : IDialogTypeLocator
  {
    const string vmSuffix = "ViewModel";

    /// <summary>
    /// Returns the type of the dialog which fits to given view model.
    /// 
    /// Currently the view model name must end with 'ViewModel' to match.
    /// 
    /// </summary>
    public Type Locate(INotifyPropertyChanged viewModel)
    {
      Type viewModelType = viewModel.GetType();
      string vmTypeName = viewModelType.FullName;

      if (vmTypeName.EndsWith("ViewModel"))
      {
        // Get the dialog name by removing the 'ViewModel' suffix and
        // then the namespace prefix
        string dialogName = vmTypeName.Substring(0, vmTypeName.Length - vmSuffix.Length);
        dialogName = dialogName.Substring(dialogName.LastIndexOf(".") + 1);

        var viewProjectAssembly = GetType().Assembly;
        foreach (Type type in viewProjectAssembly.GetTypes())
        {
          if(type.Name == dialogName)
          {
            return type;
          }
        }
      }

      return viewModelType;
    }
  }
}
