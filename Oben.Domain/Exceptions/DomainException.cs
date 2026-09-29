namespace Oben.Domain.Exceptions;

/// <summary>
/// Señala fallos de reglas de negocio sin mezclarlos con errores tecnicos
/// de transporte, base de datos o UI.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
