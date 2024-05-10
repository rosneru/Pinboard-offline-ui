using CommunityToolkit.Mvvm.Input;
using Logic.UI.Tools;
using Logic.UI.ViewModels;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using PropertyChanged;
using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Shapes;
using IOPath = System.IO.Path;

namespace Logic.UI.PageViewModels
{
  public class SettingsViewModel : PageViewModelBase
  {
    public ICommand CmdApply { get; }

    public ICommand CmdReject { get; }

    public ICommand CmdSelectFile { get; }

    public string SSHFilePath { get; set; }

    public bool IsTelnetAllowed { get; set; }

    public bool IsChanged { get; set; }

    public SettingsViewModel(UITools UiTools)
      : base("Settings", "\uE713", UiTools)
    {
      // PropertyChanged.Fody knows this flag 
      IsChanged = false;

      CmdApply = new RelayCommand(() =>
      {
        // More to come

        IsChanged = false;
      }, () => IsChanged);

      CmdReject = new RelayCommand(() =>
      {
        // More to come

        IsChanged = false;
      }, () => IsChanged);

      CmdSelectFile = new RelayCommand(() =>
      {
        var settings = new OpenFileDialogSettings
        {
          Title = "Select the SSH key",
          InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        };

        bool? success = UiTools.DialogService.ShowOpenFileDialog(this, settings);
        if (success == true)
        {
          SSHFilePath = settings.FileName;
          IsChanged = true;
        }
      }, () => true);
    }



    private void doLock()
    {
      UiTools.CmdMenuLock.Execute(null);
      IsLocked = true;
    }

    private void doUnlock()
    {
      UiTools.CmdMenuUnlock.Execute(null);
      IsLocked = false;
    }
  }
}
