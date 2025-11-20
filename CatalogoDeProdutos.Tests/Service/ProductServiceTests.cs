using AutoMapper;
using CatalogoDeProdutos.Application.DTOs;
using CatalogoDeProdutos.Application.Interfaces;
using CatalogoDeProdutos.Application.Services;
using CatalogoDeProdutos.Domain.Entities;
using CatalogoDeProdutos.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace CatalogoDeProdutos.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _mapperMock = new Mock<IMapper>();

            _productService = new ProductService(
                _productRepositoryMock.Object,
                _fileStorageServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_Should_Call_Repository_And_Return_Dto_When_Valid()
        {
            // ARRANGE
            var createDto = new CreateProductDto("Test Product", "Description", 100, "Tech", null);
            var productEntity = new Product("Test Product", "Description", 100, "Tech", null);
            var responseDto = new ProductResponseDto(productEntity.Id, "Test Product", "Description", 100, "Tech", null, "Active", DateTime.Now);

            _productRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(productEntity);
            _mapperMock.Setup(m => m.Map<ProductResponseDto>(It.IsAny<Product>())).Returns(responseDto);

            // ACT
            var result = await _productService.CreateAsync(createDto);

            // ASSERT
            Assert.NotNull(result);
            _productRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Dto_When_Product_Exists()
        {
            // ARRANGE
            var productEntity = new Product("Test Product", "Desc", 10, "Cat", null);
            var responseDto = new ProductResponseDto(productEntity.Id, "Test Product", "Desc", 10, "Cat", null, "Active", DateTime.Now);

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productEntity.Id)).ReturnsAsync(productEntity);
            _mapperMock.Setup(m => m.Map<ProductResponseDto>(productEntity)).Returns(responseDto);

            // ACT
            var result = await _productService.GetByIdAsync(productEntity.Id);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(responseDto.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Product_Does_Not_Exist()
        {
            var id = Guid.NewGuid();
            _productRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

            var result = await _productService.GetByIdAsync(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_List_Of_Dtos()
        {
            // ARRANGE
            // CORREÇÃO: Nome "Product 1" tem mais de 3 caracteres (Regra de Domínio)
            var products = new List<Product> { new Product("Product 1", "D1", 10, "C1", null) };
            var dtos = new List<ProductResponseDto> { new ProductResponseDto(Guid.NewGuid(), "Product 1", "D1", 10, "C1", null, "Active", DateTime.Now) };

            _productRepositoryMock.Setup(r => r.GetAllAsync(null, null, null, null)).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductResponseDto>>(products)).Returns(dtos);

            // ACT
            var result = await _productService.GetAllAsync(null, null, null, null);

            // ASSERT
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Entity_And_Call_Repository()
        {
            // ARRANGE
            var productEntity = new Product("Old Name", "Old Desc", 10, "Old Cat", null);
            var dto = new UpdateProductDto(productEntity.Id, "New Name", null, 20, null, null); 

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productEntity.Id)).ReturnsAsync(productEntity);

            // ACT
            await _productService.UpdateAsync(dto);

            // ASSERT
            Assert.Equal("New Name", productEntity.Name);
            Assert.Equal(20, productEntity.Price);
            _productRepositoryMock.Verify(r => r.UpdateAsync(productEntity), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Upload_New_Image_And_Delete_Old_One()
        {
            // ARRANGE
            var oldImageUrl = "http://minio/old.jpg";
            var productEntity = new Product("Product Name", "Desc", 10, "Cat", oldImageUrl);
            
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("new.jpg");
            
            var dto = new UpdateProductDto(productEntity.Id, null, null, null, null, fileMock.Object);

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productEntity.Id)).ReturnsAsync(productEntity);
            _fileStorageServiceMock.Setup(f => f.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync("http://minio/new.jpg");

            // ACT
            await _productService.UpdateAsync(dto);

            // ASSERT
            // Se este Verify falhar, seu ProductService.cs não tem a linha '_fileStorage.DeleteFileAsync' dentro do UpdateAsync
            _fileStorageServiceMock.Verify(f => f.DeleteFileAsync(oldImageUrl), Times.Once); 
            _fileStorageServiceMock.Verify(f => f.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Once);
            Assert.Equal("http://minio/new.jpg", productEntity.ImageUrl);
        }

        [Fact]
        public async Task DeleteAsync_Should_Call_Delete_On_Repository_And_Storage()
        {
            // ARRANGE
            var productEntity = new Product("Product Name", "Desc", 10, "Cat", "http://minio/img.jpg");
            _productRepositoryMock.Setup(r => r.GetByIdAsync(productEntity.Id)).ReturnsAsync(productEntity);

            // ACT
            await _productService.DeleteAsync(productEntity.Id);

            // ASSERT
            _fileStorageServiceMock.Verify(f => f.DeleteFileAsync(productEntity.ImageUrl!), Times.Once);
            _productRepositoryMock.Verify(r => r.DeleteAsync(productEntity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Throw_Exception_When_Not_Found()
        {
            var id = Guid.NewGuid();
            _productRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.DeleteAsync(id));
        }
    }
}