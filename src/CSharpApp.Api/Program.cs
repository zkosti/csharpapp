using CSharpApp.Core.Dtos;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDefaultConfiguration();
builder.Services.AddHttpConfiguration(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

var versionedEndpointRouteBuilder = app.NewVersionedApi();

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/products", async (IProductsService productsService) =>
    {
        var products = await productsService.GetProducts();
        return products;
    })
    .WithName("GetProducts")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/products/{id}", async (IProductsService productsService, int id) =>
    {
        var product = await productsService.GetOne(id);
        return product;
    })
    .WithName("GetOneProduct")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/products", async (IProductsService productsService, CreateProductRequest request) =>
    {
        var createdProduct = await productsService.Create(request);

        return Results.Created($"api/v1/products/{createdProduct?.Id}", createdProduct);
    })
    .WithName("CreateProduct")
    .HasApiVersion(1.0);
   
app.Run();