using System.Security.Cryptography;
using System.Data;
using Microsoft.Data.SqlClient;
using Oben.Application.Auth.Contracts;
using Oben.Application.Auth.Dtos;
using Oben.Domain.Entities;
using Oben.Infrastructure.Persistence;

namespace Oben.Infrastructure.Auth;

/// <summary>
/// Autentica usuarios contra la tabla users usando SQL directo.
/// La prueba define credenciales simples; hashing o JWT real pueden incorporarse sin cambiar Application.
/// </summary>
internal sealed class SqlAuthenticationService : IAuthenticationService
{
    private readonly ISqlConnectionFactory connectionFactory;

    /// <summary>
    /// Recibe la fabrica de conexiones para consultar usuarios por operacion.
    /// </summary>
    public SqlAuthenticationService(ISqlConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Busca el usuario por email y compara el password almacenado con el recibido.
    /// </summary>
    public async Task<LoginResponse?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT TOP (1) id, name, email, password
            FROM users
            WHERE email = @Email;
            """;

        command.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = email;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var user = MapUser(reader);

        if (!string.Equals(user.Password, password, StringComparison.Ordinal))
        {
            return null;
        }

        return new LoginResponse(
            user.Id,
            user.Name,
            user.Email,
            CreateAccessToken());
    }

    private static User MapUser(SqlDataReader reader)
    {
        return new User(
            reader.GetInt32(reader.GetOrdinal("id")),
            reader.GetString(reader.GetOrdinal("name")),
            reader.GetString(reader.GetOrdinal("email")),
            reader.GetString(reader.GetOrdinal("password")));
    }

    private static string CreateAccessToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}
