using Microsoft.AspNetCore.Http;

namespace CatalogoDeProdutos.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string fileName);
        Task DeleteFileAsync(string imageUrl);
    }
}