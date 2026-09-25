using System.Windows;
using MasoloAgro.App.WebView;
using MasoloAgro.App.Windows;
using MasoloAgro.Application;
using MasoloAgro.Infrastructure;
using MasoloAgro.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MasoloAgro.App;

/// <summary>
/// Application composition root. Wires dependency injection and shows the
/// main window. Business logic lives in the application and infrastructure
/// layers, never here.
/// </summary>
public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddApplication();
                services.AddInfrastructure(DatabaseLocation.GetDefaultConnectionString());
                services.AddSingleton<WebViewBridge>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        await _host.StartAsync();

        var window = _host.Services.GetRequiredService<MainWindow>();
        window.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
