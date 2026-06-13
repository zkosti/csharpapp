namespace CSharpApp.Core.Interfaces;

public interface IProductsService
{
    Task<IReadOnlyCollection<Product>> GetProducts();
    Task<Product?> GetOne(int id);
    Task<Product?> Create(ProductCreateRequest request);
}