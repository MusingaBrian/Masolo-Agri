using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MasoloAgro.App.WebView;

/// <summary>
/// Dispatches typed bridge operations from the React frontend to C# handlers.
/// Features register their operations here at startup; unknown operations
/// and unexpected failures become structured errors, never exceptions or
/// database details.
/// </summary>
public sealed class WebViewBridge
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly ILogger<WebViewBridge> _logger;
    private readonly Dictionary<string, Func<string?, CancellationToken, Task<string?>>> _handlers = new(StringComparer.Ordinal);

    public WebViewBridge(ILogger<WebViewBridge> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Registers the handler for one explicit operation name.
    /// </summary>
    public void Register(string operation, Func<string?, CancellationToken, Task<string?>> handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        ArgumentNullException.ThrowIfNull(handler);

        if (!_handlers.TryAdd(operation, handler))
        {
            throw new ArgumentException($"A handler is already registered for '{operation}'.", nameof(operation));
        }
    }

    /// <summary>
    /// Handles one raw JSON message received from the WebView and returns
    /// the JSON response to post back. Messages use an envelope carrying
    /// the caller id so concurrent calls receive the right reply:
    /// { id, request: { operation, payload } }.
    /// </summary>
    public async Task<string> DispatchJsonAsync(string? message, CancellationToken cancellationToken = default)
    {
        int id = 0;
        BridgeRequest? request = null;
        try
        {
            using var document = JsonDocument.Parse(message ?? string.Empty);
            if (document.RootElement.TryGetProperty("id", out var idElement)
                && idElement.TryGetInt32(out var parsedId)
                && document.RootElement.TryGetProperty("request", out var requestElement))
            {
                id = parsedId;
                request = requestElement.Deserialize<BridgeRequest>(JsonOptions);
            }
        }
        catch (JsonException)
        {
            request = null;
        }

        BridgeResponse response = request is null || string.IsNullOrWhiteSpace(request.Operation)
            ? new BridgeResponse(false, null, "The request was not understood.")
            : await DispatchAsync(request, cancellationToken);

        return JsonSerializer.Serialize(new { id, response }, JsonOptions);
    }

    public async Task<BridgeResponse> DispatchAsync(BridgeRequest request, CancellationToken cancellationToken = default)
    {
        if (!_handlers.TryGetValue(request.Operation, out var handler))
        {
            return new BridgeResponse(false, null, $"Unknown operation '{request.Operation}'.");
        }

        try
        {
            var payload = await handler(request.Payload, cancellationToken);
            return new BridgeResponse(true, payload, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bridge operation '{Operation}' failed.", request.Operation);
            return new BridgeResponse(false, null, "The request could not be completed.");
        }
    }
}
