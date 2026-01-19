using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config.Net;

namespace Logic.UI.Model
{
  public interface IAppSettings : INotifyPropertyChanged
  {
    public string JSONFileURL { get; set; }
    public bool AskBeforeAppExit { get; set; }
    public ThemeType ReaderTheme { get; set; }
  }
}
