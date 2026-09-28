namespace Oben.Application.Common.Exceptions;

/// <summary>
/// Senala que las credenciales o la autorizacion no permiten completar el caso de uso.
/// La API puede traducirla directamente a HTTP 401.
/// </summary>
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}
