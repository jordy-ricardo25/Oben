namespace Oben.Application.Common.Exceptions;

/// <summary>
/// Senala que un caso de uso necesita un recurso que no existe.
/// La API puede traducirla directamente a HTTP 404.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
