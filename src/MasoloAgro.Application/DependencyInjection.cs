using Microsoft.Extensions.DependencyInjection;

namespace MasoloAgro.Application;

/// <summary>
/// Composition root for application services. Feature services
/// (auth, commodities, sales, ...) register here as they are built.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
