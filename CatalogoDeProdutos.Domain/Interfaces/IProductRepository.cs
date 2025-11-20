using CatalogoDeProdutos.Domain.Entities;
namespace CatalogoDeProdutos.Domain.Interfaces
{
    public interface IProductRepository
    {
        // CRUD Básico
        Task<Product> CreateAsync(Product product);
        Task<Product?> GetByIdAsync(Guid id);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product); 

        Task<IEnumerable<Product>> GetAllAsync(
            string? category = null, 
            decimal? minPrice = null, 
            decimal? maxPrice = null, 
            ProductStatus? status = null
        );
    }
}