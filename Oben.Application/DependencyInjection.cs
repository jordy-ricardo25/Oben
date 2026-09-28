using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Oben.Application.Auth.Commands.Login;
using Oben.Application.Common.Behaviors;
using Oben.Application.Products.Commands.CreateProduct;
using Oben.Application.Products.Commands.DeleteProduct;
using Oben.Application.Products.Commands.UpdateProduct;
using Oben.Application.Products.Queries.GetProductById;

namespace Oben.Application;

/// <summary>
/// Punto unico de registro para los servicios propios de la capa de aplicacion.
/// Mantiene a API y MAUI libres de detalles sobre MediatR, validacion y behaviors.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra casos de uso y validacion transversal. Los contratos que dependen
    /// de SQL Server o seguridad concreta se implementan desde infraestructura.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddTransient<IValidator<LoginCommand>, LoginCommandValidator>();
        services.AddTransient<IValidator<CreateProductCommand>, CreateProductCommandValidator>();
        services.AddTransient<IValidator<UpdateProductCommand>, UpdateProductCommandValidator>();
        services.AddTransient<IValidator<DeleteProductCommand>, DeleteProductCommandValidator>();
        services.AddTransient<IValidator<GetProductByIdQuery>, GetProductByIdQueryValidator>();

        return services;
    }
}
