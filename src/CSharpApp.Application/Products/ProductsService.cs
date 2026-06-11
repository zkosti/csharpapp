using System.Net;
using System.Net.Http.Json;

namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(HttpClient httpClient, IOptions<RestApiSettings> restApiSettings,
        ILogger<ProductsService> logger)
    {
        _httpClient = httpClient;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        var response = await _httpClient.GetAsync(_restApiSettings.Products);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Product>>(content);

        return (res ?? []).AsReadOnly();
    }

    public async Task<Product?> GetOne(int id)
    {
        var response = await _httpClient.GetAsync($"{_restApiSettings.Products}/{id}");

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogInformation("Product with ID {ProductId} not found.", id);
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task<Product?> Create(CreateProductRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(_restApiSettings.Products, request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("External API error: {Error}", error);

            return null;
        }

        var product = await response.Content.ReadFromJsonAsync<Product>();
        _logger.LogInformation("Product created with ID: {ProductId}", product?.Id);

        return product;
    }

}