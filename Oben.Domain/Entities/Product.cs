namespace Oben.Domain.Entities;

using Oben.Domain.Exceptions;

/// <summary>
/// Mantiene las invariantes de producto que deben cumplirse aunque el objeto
/// se cree desde la UI, un handler o el mapeo manual de SQL.
/// </summary>
public sealed class Product
{
    /// <summary>
    /// Crea un producto valido. El identificador puede ser cero mientras SQL Server
    /// aun no haya asignado el valor definitivo.
    /// </summary>
    public Product(int id, string name, string? description, decimal price, int stock)
    {
        ValidateId(id);
        ValidateName(name);
        ValidatePrice(price);
        ValidateStock(stock);

        Id = id;
        Name = name.Trim();
        Description = NormalizeDescription(description);
        Price = price;
        Stock = stock;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    /// <summary>
    /// Actualiza todos los campos editables pasando por las mismas reglas que la creacion.
    /// </summary>
    public void Update(string name, string? description, decimal price, int stock)
    {
        ValidateName(name);
        ValidatePrice(price);
        ValidateStock(stock);

        Name = name.Trim();
        Description = NormalizeDescription(description);
        Price = price;
        Stock = stock;
    }

    private static void ValidateId(int id)
    {
        if (id < 0)
        {
            throw new DomainException("Product id cannot be negative.");
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name is required.");
        }
    }

    private static void ValidatePrice(decimal price)
    {
        if (price <= 0)
        {
            throw new DomainException("Product price must be greater than zero.");
        }
    }

    private static void ValidateStock(int stock)
    {
        if (stock < 0)
        {
            throw new DomainException("Product stock cannot be negative.");
        }
    }

    /// <summary>
    /// Evita que una descripcion vacia llegue a persistencia como un valor distinto de null.
    /// </summary>
    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
