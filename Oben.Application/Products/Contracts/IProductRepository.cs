using Oben.Domain.Entities;

namespace Oben.Application.Products.Contracts;

/// <summary>
/// Puerto de persistencia para productos usado por los casos de uso.
/// La implementacion debe usar SQL directo y mapeo manual desde infraestructura.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Persiste un producto nuevo y devuelve la entidad con el identificador asignado por SQL Server.
    /// </summary>
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken);

    /// <summary>
    /// Actualiza un producto existente. La implementacion debe establecer SESSION_CONTEXT(N'UserId')
    /// en la misma conexion antes de ejecutar el UPDATE para que los triggers auditen correctamente.
    /// </summary>
    Task UpdateAsync(Product product, int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Elimina un producto existente. La implementacion debe establecer SESSION_CONTEXT(N'UserId')
    /// en la misma conexion antes de ejecutar el DELETE para que los triggers auditen correctamente.
    /// </summary>
    Task DeleteAsync(int id, int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Busca un producto por identificador y devuelve null cuando no existe.
    /// </summary>
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Obtiene el listado completo de productos requerido por la prueba tecnica.
    /// </summary>
    Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken);
}

