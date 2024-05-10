using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmDialogs;

namespace Logic.UI.DialogViewModels
{
  public class DeviceFile
  {
    public string FileName { get; private set; }
    public string IPAddress { get; private set; }

    public DeviceFile(string fileName, string iPAddress)
    {
      FileName = fileName;
      IPAddress = iPAddress;
    }
  }

  public class OpenDeviceDialogViewModel : ObservableObject, IModalDialogViewModel
  {
    public bool? DialogResult => dialogResult;

    public List<DeviceFile> AvailableDevices { get; private set; }
    public DeviceFile SelectedDeviceFile { get; set; }

    public ICommand CmdLoad { get; }
    public ICommand CmdCancel { get; }

    public string FilterText { get; set; }

    public OpenDeviceDialogViewModel(IDialogService dialogService)
    {
      AvailableDevices = new List<DeviceFile>()
      {
        new DeviceFile("First device file", "192.168.210.141"),
        new DeviceFile("Second device file", "192.168.210.142"),
        new DeviceFile("Third device file", "192.168.210.143"),
      };

      CmdLoad = new RelayCommand(() =>
      {
        dialogResult = true;
        dialogService.Close(this);
      }, () => SelectedDeviceFile != null);

      CmdCancel = new RelayCommand(() =>
      {
        dialogResult = false;
        dialogService.Close(this);
      }, () => true);
    }

    private bool? dialogResult;
  }
}
