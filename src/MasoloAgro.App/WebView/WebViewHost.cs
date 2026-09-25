using System.IO;
using Microsoft.Web.WebView2.Wpf;

namespace MasoloAgro.App.WebView;

/// <summary>
/// Starts the embedded browser: initializes WebView2, wires incoming
/// messages to the typed bridge, then loads the frontend. In development
/// builds the Vite dev server is used; production builds load the bundled
/// frontend from the application folder.
/// </summary>
public static class WebViewHost
{
    /// <summary>
    /// Local address of the Vite development server.
    /// </summary>
    public const string DevServerUrl = "http://localhost:5173";

    public static Uri GetFrontendUri()
    {
#if DEBUG
        return new Uri(DevServerUrl);
#else
        return new Uri(Path.Combine(AppContext.BaseDirectory, "wwwroot", "index.html"));
#endif
    }

    public static async Task InitializeAsync(WebView2 webView, WebViewBridge bridge, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(webView);
        ArgumentNullException.ThrowIfNull(bridge);

        await webView.EnsureCoreWebView2Async();

        webView.CoreWebView2.WebMessageReceived += async (_, e) =>
        {
            var response = await bridge.DispatchJsonAsync(e.TryGetWebMessageAsString(), cancellationToken);
            webView.CoreWebView2.PostWebMessageAsJson(response);
        };

        webView.Source = GetFrontendUri();
    }
}
