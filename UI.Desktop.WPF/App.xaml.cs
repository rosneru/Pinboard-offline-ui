using CommunityToolkit.Mvvm.DependencyInjection;
using Logic.UI;
using Logic.UI.DialogViewModels;
using Logic.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using System;
using System.Windows;

namespace UI.Desktop.WPF
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// 
  /// Note: setup the services / view models below has been done according to this:
  /// https://docs.microsoft.com/en-us/windows/communitytoolkit/mvvm/ioc?source=recommendations
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      Services = ConfigureServices();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      MainWindow app = new MainWindow();

      app.DataContext = Services.GetRequiredService<MainViewModel>();
      app.Show();
    }

    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;


    public IServiceProvider Services { get; }


    private static IServiceProvider ConfigureServices()
    {
      var services = new ServiceCollection();

      var dialogService = new DialogService(dialogTypeLocator: new DialogTypeLocator());
      services.AddSingleton<IDialogService, DialogService>(_ => dialogService).AddTransient<MainViewModel>();

      return services.BuildServiceProvider();
    }
  }
}
