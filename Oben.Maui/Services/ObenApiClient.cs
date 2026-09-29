using System.Net;
using System.Net.Http.Json;
using Oben.Maui.Services.Models;

namespace Oben.Maui.Services;

/// <summary>
/// Cliente HTTP pequeno para consumir Oben.Api desde Blazor Hybrid.
/// </summary>
public sealed class ObenApiClient
{
    private readonly HttpClient httpClient;

    public ObenApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<LoginResponse> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            "api/auth/login",
            new { email, password },
            cancellationToken);

        await EnsureSuccessAsync(response, "No pudimos iniciar sesion.", cancellationToken);

        return await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken)
            ?? throw new InvalidOperationException("La API no devolvio datos de sesion.");
    }

    public async Task<IReadOnlyCollection<ProductDto>> GetProductsAsync(CancellationToken cancellationToken)
    {
        return await httpClient.GetFromJsonAsync<IReadOnlyCollection<ProductDto>>(
            "api/products",
            cancellationToken) ?? [];
    }

    public async Task<ProductDto> CreateProductAsync(
        ProductFormModel product,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/products", product, cancellationToken);

        await EnsureSuccessAsync(response, "No pudimos crear el producto.", cancellationToken);

        return await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken)
            ?? throw new InvalidOperationException("La API no devolvio el producto creado.");
    }

    public async Task<ProductDto> UpdateProductAsync(
        int id,
        int userId,
        ProductFormModel product,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/products/{id}")
        {
            Content = JsonContent.Create(product)
        };

        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await httpClient.SendAsync(request, cancellationToken);

        await EnsureSuccessAsync(response, "No pudimos actualizar el producto.", cancellationToken);

        return await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken)
            ?? throw new InvalidOperationException("La API no devolvio el producto actualizado.");
    }

    public async Task DeleteProductAsync(int id, int userId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/products/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await httpClient.SendAsync(request, cancellationToken);

        await EnsureSuccessAsync(response, "No pudimos eliminar el producto.", cancellationToken);
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        string fallbackMessage,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new InvalidOperationException("Credenciales invalidas.");
        }

        throw new InvalidOperationException(fallbackMessage);
    }
}
