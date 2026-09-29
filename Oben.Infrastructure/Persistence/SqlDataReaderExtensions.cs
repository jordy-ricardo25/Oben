using Microsoft.Data.SqlClient;

namespace Oben.Infrastructure.Persistence;

/// <summary>
/// Utilidades pequenas para mantener legible el mapeo manual desde SqlDataReader.
/// </summary>
internal static class SqlDataReaderExtensions
{
    /// <summary>
    /// Lee un string nullable respetando valores NULL de SQL Server.
    /// </summary>
    public static string? GetNullableString(this SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);

        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }
}
