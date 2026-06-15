using CSharpApp.Application.AuthService;
using CSharpApp.Application.Categories;

namespace CSharpApp.Infrastructure.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new Uri(configuration["RestApiSettings:BaseUrl"]!);
        });

        services.AddHttpClient<IProductsService, ProductsService>(client =>
        {
            client.BaseAddress = new Uri(configuration["RestApiSettings:BaseUrl"]!);
        });

        services.AddHttpClient<ICategoriesService, CategoriesService>(client =>
        {
            client.BaseAddress = new Uri(configuration["RestApiSettings:BaseUrl"]!);
        });

        return services;
    }
}