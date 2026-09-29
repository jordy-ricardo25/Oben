using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oben.Application.Auth.Contracts;
using Oben.Application.Products.Contracts;
using Oben.Infrastructure.Auth;
using Oben.Infrastructure.Persistence;
using Oben.Infrastructure.Products;

namespace Oben.Infrastructure;

/// <summary>
/// Registra las implementaciones concretas de infraestructura sin exponer SQL Server
/// a las capas de aplicacion, dominio o UI.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Conecta los puertos de aplicacion con implementaciones basadas en SQL directo.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is required for Oben.Infrastructure.");
        }

        services.AddSingleton<ISqlConnectionFactory>(
            new SqlConnectionFactory(connectionString));

        services.AddScoped<IAuthenticationService, SqlAuthenticationService>();
        services.AddScoped<IProductRepository, SqlProductRepository>();

        return services;
    }
}
