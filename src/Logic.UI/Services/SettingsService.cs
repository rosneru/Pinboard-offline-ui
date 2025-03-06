using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Config.Net;
using Logic.UI.Model;

namespace Logic.UI.Services
{
  public partial class SettingsService : ObservableObject, ISettingsService
  {
    [ObservableProperty] private string _pinboardFileName;
    [ObservableProperty] private string _appSettingsPath;
    [ObservableProperty] private IAppSettings _appSettings;

    public SettingsService()
    {
      PinboardFileName = "pinboard_backup.json";
      var appDataRoamingPath = Environment
        .GetFolderPath(Environment.SpecialFolder.ApplicationData);
      var developerName = "tysw";
      var appName = "Pinboard-offline-ui";

      AppSettingsPath = Path.Combine(appDataRoamingPath, developerName, appName);

      if (!Directory.Exists(_appSettingsPath))
      {
        Directory.CreateDirectory(_appSettingsPath);
      }

      var appSettingsFile = Path.Combine(_appSettingsPath, "settings.json");
      AppSettings = new ConfigurationBuilder<IAppSettings>()
        .UseJsonFile(appSettingsFile)
        .Build();
    }
  }
}
