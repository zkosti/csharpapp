using System.Net;
using System.Net.Http.Json;

namespace CSharpApp.Application.Categories;

public class CategoriesService : ICategoriesService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<CategoriesService> _logger;

    public CategoriesService(HttpClient httpClient, IOptions<RestApiSettings> restApiSettings,
        ILogger<CategoriesService> logger)
    {
        _httpClient = httpClient;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {
        var response = await _httpClient.GetAsync(_restApiSettings.Categories);
        response.EnsureSuccessStatusCode();

        var categories = await response.Content.ReadFromJsonAsync<List<Category>>();

        return (categories ?? []).AsReadOnly();
    }

    public async Task<Category?> GetOne(int id)
    {
        var response = await _httpClient.GetAsync($"{_restApiSettings.Categories}/{id}");

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogInformation("Category with ID {CategoryId} not found.", id);
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Category>();
    }

    public async Task<Category?> Create(CategoryCreateRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(_restApiSettings.Categories, request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("External API error: {Error}", error);

            return null;
        }

        var product = await response.Content.ReadFromJsonAsync<Category>();
        _logger.LogInformation("Category created with ID: {CategoryId}", product?.Id);

        return product;
    }

}