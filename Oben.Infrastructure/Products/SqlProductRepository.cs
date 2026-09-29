using System.Data;
using Microsoft.Data.SqlClient;
using Oben.Application.Products.Contracts;
using Oben.Domain.Entities;
using Oben.Infrastructure.Persistence;

namespace Oben.Infrastructure.Products;

/// <summary>
/// Implementa la persistencia de productos con SQL directo y mapeo manual.
/// No usa Entity Framework ni mapeadores automaticos por restriccion del proyecto.
/// </summary>
internal sealed class SqlProductRepository : IProductRepository
{
    private readonly ISqlConnectionFactory connectionFactory;

    /// <summary>
    /// Recibe la fabrica de conexiones para abrir una conexion por operacion.
    /// </summary>
    public SqlProductRepository(ISqlConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Inserta un producto y devuelve la entidad con el id asignado por SQL Server.
    /// </summary>
    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();

        command.CommandText = """
            INSERT INTO products (name, description, price, stock)
            OUTPUT INSERTED.id
            VALUES (@Name, @Description, @Price, @Stock);
            """;

        AddProductParameters(command, product);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        var id = Convert.ToInt32(result);

        return new Product(
            id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock);
    }

    /// <summary>
    /// Actualiza un producto y configura el usuario de auditoria en la misma conexion del UPDATE.
    /// </summary>
    public async Task UpdateAsync(Product product, int userId, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

        await SetSessionUserAsync(connection, userId, cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE products
            SET name = @Name,
                description = @Description,
                price = @Price,
                stock = @Stock
            WHERE id = @Id;
            """;

        command.Parameters.Add("@Id", SqlDbType.Int).Value = product.Id;
        AddProductParameters(command, product);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina un producto y configura el usuario de auditoria en la misma conexion del DELETE.
    /// </summary>
    public async Task DeleteAsync(int id, int userId, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

        await SetSessionUserAsync(connection, userId, cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            DELETE FROM products
            WHERE id = @Id;
            """;

        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Busca un producto por id y devuelve null cuando SQL Server no retorna filas.
    /// </summary>
    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT id, name, description, price, stock
            FROM products
            WHERE id = @Id;
            """;

        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return MapProduct(reader);
    }

    /// <summary>
    /// Obtiene todos los productos ordenados por la columna name para entregar un listado estable a la UI.
    /// </summary>
    public async Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT id, name, description, price, stock
            FROM products
            ORDER BY name;
            """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var products = new List<Product>();

        while (await reader.ReadAsync(cancellationToken))
        {
            products.Add(MapProduct(reader));
        }

        return products;
    }

    private static void AddProductParameters(SqlCommand command, Product product)
    {
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = product.Name;
        command.Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value =
            (object?)product.Description ?? DBNull.Value;
        var priceParameter = command.Parameters.Add("@Price", SqlDbType.Decimal);
        priceParameter.Precision = 18;
        priceParameter.Scale = 2;
        priceParameter.Value = product.Price;
        command.Parameters.Add("@Stock", SqlDbType.Int).Value = product.Stock;
    }

    private static Product MapProduct(SqlDataReader reader)
    {
        return new Product(
            reader.GetInt32(reader.GetOrdinal("id")),
            reader.GetString(reader.GetOrdinal("name")),
            reader.GetNullableString("description"),
            reader.GetDecimal(reader.GetOrdinal("price")),
            reader.GetInt32(reader.GetOrdinal("stock")));
    }

    private static async Task SetSessionUserAsync(
        SqlConnection connection,
        int userId,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            EXEC sp_set_session_context @key = N'UserId', @value = @UserId;
            """;

        command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
