using CatalogoDeProdutos.Application.DTOs;
using CatalogoDeProdutos.Domain;

namespace CatalogoDeProdutos.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
        Task<ProductResponseDto?> GetByIdAsync(Guid id);
        Task UpdateAsync(UpdateProductDto dto);
        Task DeleteAsync(Guid id);
        
        Task<IEnumerable<ProductResponseDto>> GetAllAsync(
            string? category = null, 
            decimal? minPrice = null, 
            decimal? maxPrice = null, 
            ProductStatus? status = null
        );
    }
}