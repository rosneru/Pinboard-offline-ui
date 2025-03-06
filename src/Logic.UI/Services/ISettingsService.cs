using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.UI.Model;

namespace Logic.UI.Services
{
  public interface ISettingsService
  {
    string PinboardFileName { get; }
    string AppSettingsPath { get; }
    IAppSettings AppSettings { get; }
  }
}
