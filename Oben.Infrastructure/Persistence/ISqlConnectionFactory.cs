using Microsoft.Data.SqlClient;

namespace Oben.Infrastructure.Persistence;

/// <summary>
/// Crea conexiones abiertas contra SQL Server para cada operacion de infraestructura.
/// </summary>
internal interface ISqlConnectionFactory
{
    /// <summary>
    /// Abre una conexion nueva. Quien la recibe es responsable de liberarla con await using.
    /// </summary>
    Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken);
}
