using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using CSharpApp.Application.Products;
using CSharpApp.Core.Settings;
using CSharpApp.Core.Dtos;

namespace CSharpApp.Tests;

public class UnitTestProductService
{
    [Fact]
    public async Task GetProducts_ReturnsAllProducts_WhenApiReturnsOk()
    {
        var products = """
                    [
                        {
                            "id": 1,
                            "title": "Test Product 1",
                            "price": 73,
                            "description": "text",
                            "images": [],
                            "creationAt": "2026-06-10T22:12:20Z",
                            "updatedAt": "2026-06-10T22:12:20Z",
                            "category": {
                                "id": 1,
                                "name": "Category",
                                "image": "https://i.imgur.com/test.jpg",
                                "creationAt": "2026-06-10T22:12:20Z",
                                "updatedAt": "2026-06-11T16:00:44Z"
                            }
                        },
                        {
                            "id": 2,
                            "title": "Test Product 2",
                            "price": 73,
                            "description": "text",
                            "images": [],
                            "creationAt": "2026-06-10T22:12:20Z",
                            "updatedAt": "2026-06-10T22:12:20Z",
                            "category": {
                                "id": 1,
                                "name": "Category",
                                "image": "https://i.imgur.com/test.jpg",
                                "creationAt": "2026-06-10T22:12:20Z",
                                "updatedAt": "2026-06-11T16:00:44Z"
                            }
                        }
                    ]
                    """;

        var response = CommonServices.CreateHttpResponse(products, HttpStatusCode.OK);
        var service = CreateService(response);

        var result = await service.GetProducts();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetOne_ReturnsProduct_WhenApiReturnsOk()
    {
        var product = """
                    {
                        "id": 1,
                        "title": "Test Product 1",
                        "price": 73,
                        "description": "text",
                        "images": [],
                        "creationAt": "2026-06-10T22:12:20Z",
                        "updatedAt": "2026-06-10T22:12:20Z",
                        "category": {
                            "id": 1,
                            "name": "Category",
                            "image": "https://i.imgur.com/test.jpg",
                            "creationAt": "2026-06-10T22:12:20Z",
                            "updatedAt": "2026-06-11T16:00:44Z"
                        }
                    }
                    """;

        var response = CommonServices.CreateHttpResponse(product, HttpStatusCode.OK);
        var service = CreateService(response);

        var result = await service.GetOne(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetOne_ReturnsNull_WhenApiReturnsProductNotFound()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var service = CreateService(response);
        
        var result = await service.GetOne(999);

        Assert.Null(result);
    }


    [Fact]
    public async Task Create_ReturnsCreatedProduct_WhenApiReturnsOk()
    {
        var request = new ProductCreateRequest
        {
            Title = "A Test Product",
            Price = 10,
            Description = "This is a test description",
            CategoryId = 1,
            Images = ["https://placeimg.com/640/480/any"]
        };

        var createdProduct = """
                        {
                            "id": 999,
                            "title": "A Test Product",
                            "price": 10,
                            "description": "This is a test description",
                            "images": [],
                            "creationAt": "2026-06-11T19:00:59Z",
                            "updatedAt": "2026-06-11T19:00:59Z",
                            "category": {
                                "id": 1,
                                "name": "Test Category",
                                "image": "https://placeimg.com/640/480/any",
                                "creationAt": "2026-06-10T22:12:20Z",
                                "updatedAt": "2026-06-11T18:02:56Z"
                            }
                        }
                        """;

        var response = CommonServices.CreateHttpResponse(createdProduct, HttpStatusCode.Created);
        var service = CreateService(response);
        var result = await service.Create(request);

        Assert.NotNull(result);
        Assert.Equal(999, result.Id);
        Assert.Equal("A Test Product", result.Title);
    }

    [Fact]
    public async Task Create_ReturnsNull_WhenApiReturnsError()
    {
        var request = new ProductCreateRequest
        {
            Title = "A Test Product",
            Price = 10,
            Description = "This is a test description",
            CategoryId = 1,
            Images = new List<string> { "https://placeimg.com/640/480/any" }
        };

        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        var service = CreateService(response);
        var result = await service.Create(request);

        Assert.Null(result);
    }

    private static ProductsService CreateService(HttpResponseMessage response) // Helper method to create Service with mocked HttpClient
    {
        var handler = new CommonServices.HttpMessageHandlerStub(response);

        var options = Options.Create(new RestApiSettings
        {
            BaseUrl = "http://testapi/v1/",
            Products = "products"
        });

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(options.Value.BaseUrl!)
        };

        var logger = NullLogger<ProductsService>.Instance;

        return new ProductsService(httpClient, options, logger);
    }
}
