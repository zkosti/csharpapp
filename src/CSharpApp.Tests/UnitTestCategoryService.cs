using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using CSharpApp.Core.Settings;
using CSharpApp.Core.Dtos;
using CSharpApp.Application.Categories;

namespace CSharpApp.Tests;

public class UnitTestCategoryService
{
    [Fact]
    public async Task GetCategories_ReturnsAllProducts_WhenApiReturnsOk()
    {
        var categories = """
                    [
                        {
                            "id": 1,
                            "name": "Test Books",
                            "image": "https://example.com/updatedimage.jpg",
                            "creationAt": "2026-06-11T22:34:27Z",
                            "updatedAt": "2026-06-12T05:50:53Z"
                        },
                        {
                            "id": 2,
                            "name": "Test Electronics",
                            "image": "https://example.com/updatedelectronics.jpg",
                            "creationAt": "2026-06-11T22:34:27Z",
                            "updatedAt": "2026-06-11T23:08:09Z"
                        }
                    ]
                    """;

        var response = CommonServices.CreateHttpResponse(categories, HttpStatusCode.OK);
        var service = CreateService(response);

        var result = await service.GetCategories();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetOne_ReturnsCategories_WhenApiReturnsOk()
    {
        var category = """
                        {
                            "id": 1,
                            "name": "Test Books",
                            "image": "https://example.com/updatedimage.jpg",
                            "creationAt": "2026-06-11T22:34:27Z",
                            "updatedAt": "2026-06-12T05:50:53Z"
                        }
                        """;

        var response = CommonServices.CreateHttpResponse(category, HttpStatusCode.OK);
        var service = CreateService(response);

        var result = await service.GetOne(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetOne_ReturnsNull_WhenApiReturnsNotFound()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var service = CreateService(response);

        var result = await service.GetOne(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedCategory_WhenApiReturnsOk()
    {
        var request = new CategoryCreateRequest
        {
            Name = "Test Books",
            Image = "https://api.lorem.space/image/book?w=150&h=220"

        };

        var createdCategory = """
                            { 
                                "id": 999,
                                "name": "Test Books",
                                "image": "https://api.lorem.space/image/book?w=150&h=220",
                                "creationAt": "2026-06-12T09:12:00Z",
                                "updatedAt": "2026-06-12T09:12:00Z"
                            }
                            """;

        var response = CommonServices.CreateHttpResponse(createdCategory, HttpStatusCode.Created);
        var service = CreateService(response);

        var result = await service.Create(request);

        Assert.NotNull(result);
        Assert.Equal(999, result.Id);
        Assert.Equal("Test Books", result.Name);
    }

    [Fact]
    public async Task Create_ReturnsNull_WhenApiReturnsError()
    {
        var request = new CategoryCreateRequest
        {
            Name = "Test Books",
            Image = "https://api.lorem.space/image/book?w=150&h=220"

        };

        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        var service = CreateService(response);

        var result = await service.Create(request);

        Assert.Null(result);
    }

    private static CategoriesService CreateService(HttpResponseMessage response)
    {
        var handler = new CommonServices.HttpMessageHandlerStub(response);

        var options = Options.Create(new RestApiSettings
        {
            BaseUrl = "http://testapi/v1/",
            Categories = "categories"
        });

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(options.Value.BaseUrl!)
        };

        var logger = NullLogger<CategoriesService>.Instance;

        return new CategoriesService(httpClient, options, logger);
    }
}
