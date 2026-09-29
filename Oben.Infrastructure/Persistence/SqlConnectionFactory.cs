using Microsoft.Data.SqlClient;

namespace Oben.Infrastructure.Persistence;

/// <summary>
/// Implementacion minima de fabrica de conexiones SQL Server.
/// Centraliza el uso del connection string sin ocultar el acceso SQL directo.
/// </summary>
internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string connectionString;

    /// <summary>
    /// Recibe el connection string validado durante el registro de infraestructura.
    /// </summary>
    public SqlConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    /// <summary>
    /// Crea y abre una conexion nueva para una unica operacion de repositorio o autenticacion.
    /// </summary>
    public async Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        return connection;
    }
}
