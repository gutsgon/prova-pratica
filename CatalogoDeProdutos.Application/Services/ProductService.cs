using AutoMapper;
using CatalogoDeProdutos.Application.DTOs;
using CatalogoDeProdutos.Application.Interfaces;
using CatalogoDeProdutos.Domain.Entities;
using CatalogoDeProdutos.Domain.Interfaces;
using CatalogoDeProdutos.Domain;

namespace CatalogoDeProdutos.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IFileStorageService _fileStorage;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IFileStorageService fileStorage, IMapper mapper)
        {
            _repository = repository;
            _fileStorage = fileStorage;
            _mapper = mapper;
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
        {
            string? imageUrl = null;

            if (dto.ImageFile != null)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.ImageFile.FileName}";
                imageUrl = await _fileStorage.UploadFileAsync(dto.ImageFile, fileName);
            }

            var product = new Product(dto.Name, dto.Description, dto.Price, dto.Category, imageUrl);

            var createdProduct = await _repository.CreateAsync(product);

            return _mapper.Map<ProductResponseDto>(createdProduct);
        }

        public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return null;
            
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(string? category, decimal? minPrice, decimal? maxPrice, ProductStatus? status)
        {
            var products = await _repository.GetAllAsync(category, minPrice, maxPrice, status);
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task UpdateAsync(UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException("Product not found");
            var name = dto.Name ?? product.Name;
            var description = dto.Description ?? product.Description;
            var price = dto.Price ?? product.Price; 
            var category = dto.Category ?? product.Category;

            product.Update(name, description, price, category);

            if (dto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    await _fileStorage.DeleteFileAsync(product.ImageUrl);
                }
                var fileName = $"{Guid.NewGuid()}_{dto.ImageFile.FileName}";
                var newImageUrl = await _fileStorage.UploadFileAsync(dto.ImageFile, fileName);
                product.UpdateImage(newImageUrl);
            }

            await _repository.UpdateAsync(product);
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found");

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                await _fileStorage.DeleteFileAsync(product.ImageUrl);
            }
            await _repository.DeleteAsync(product);
        }
    }
}