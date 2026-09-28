using FluentValidation;
using MediatR;

namespace Oben.Application.Common.Behaviors;

/// <summary>
/// Ejecuta los validadores de FluentValidation antes de llamar al handler del caso de uso.
/// Centralizar esta regla evita repetir validaciones defensivas en cada command o query.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    /// <summary>
    /// Recibe los validadores registrados para el tipo de request que MediatR esta procesando.
    /// </summary>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    /// <summary>
    /// Continua el pipeline cuando la entrada es valida; en caso contrario lanza
    /// <see cref="ValidationException"/> para que la capa API decida la respuesta HTTP.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestValidators = validators.ToList();

        if (requestValidators.Count == 0)
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            requestValidators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}

