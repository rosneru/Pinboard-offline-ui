using CommunityToolkit.Mvvm.DependencyInjection;
using Logic.UI;
using Logic.UI.DialogViewModels;
using Logic.UI.ViewModels;
using Microsoft.Extensions.Hosting;
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
    public IHost Host => _host.Value;


    public App()
    {
      _host = new(InitializeHost);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      MainWindow app = new MainWindow();

      app.DataContext = GetService<MainViewModel>();
      app.Show();
    }

    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;


    private IHost InitializeHost()
    {
      return Microsoft.Extensions.Hosting.Host
        .CreateDefaultBuilder()
        .UseContentRoot(AppContext.BaseDirectory)
        .ConfigureServices(ConfigureServices)
        .Build();
    }


    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
      services
        .AddSingleton<IDialogService, DialogService>(_ => new(dialogTypeLocator: new DialogTypeLocator()))
        .AddTransient<MainViewModel>();
    }

    public static T GetService<T>() where T : class
    {
      if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
      {
        throw new InvalidOperationException($"Type {typeof(T).Name} is not configured");
      }

      return service;
    }


    private readonly Lazy<IHost> _host;
  }
}
