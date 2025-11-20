using Microsoft.AspNetCore.Http;

namespace CatalogoDeProdutos.Application.DTOs
{
    public record CreateProductDto(
        string Name, 
        string Description, 
        decimal Price, 
        string Category, 
        IFormFile? ImageFile 
    );

    public record UpdateProductDto(
        Guid Id,
        string? Name, 
        string? Description, 
        decimal? Price, 
        string? Category,
        IFormFile? ImageFile 
    );

    public record ProductResponseDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        string Category,
        string? ImageUrl,
        string Status,
        DateTime CreatedAt
    );
}