namespace CSharpApp.Core.Interfaces;

public interface ICategoriesService
{
    Task<IReadOnlyCollection<Category>> GetCategories();
    Task<Category?> GetOne(int id);
    Task<Category?> Create(CategoryCreateRequest request);
}