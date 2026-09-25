using System.Windows;
using MasoloAgro.App.WebView;
using Microsoft.Extensions.Logging;

namespace MasoloAgro.App.Windows;

/// <summary>
/// Main application window. Hosts the WebView2 control; all business UI
/// lives in the React frontend.
/// </summary>
public partial class MainWindow : Window
{
    private readonly WebViewBridge _bridge;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(WebViewBridge bridge, ILogger<MainWindow> logger)
    {
        _bridge = bridge;
        _logger = logger;
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await WebViewHost.InitializeAsync(MainWebView, _bridge);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start the embedded browser.");
            FallbackText.Visibility = Visibility.Visible;
        }
    }
}
