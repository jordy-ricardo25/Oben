namespace Oben.Domain.Entities;

using Oben.Domain.Exceptions;

/// <summary>
/// Modelo minimo de usuario requerido por el login de la prueba tecnica.
/// </summary>
public sealed class User
{
    /// <summary>
    /// Crea un usuario valido. La contrasena se conserva como dato recibido; hashing,
    /// comparacion segura y politicas de credenciales pertenecen a aplicacion/infraestructura.
    /// </summary>
    public User(int id, string name, string email, string password)
    {
        ValidateId(id);
        ValidateName(name);
        ValidateEmail(email);
        ValidatePassword(password);

        Id = id;
        Name = name.Trim();
        Email = email.Trim();
        Password = password;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public string Password { get; private set; }

    private static void ValidateId(int id)
    {
        if (id < 0)
        {
            throw new DomainException("User id cannot be negative.");
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("User name is required.");
        }
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("User email is required.");
        }
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new DomainException("User password is required.");
        }
    }
}
