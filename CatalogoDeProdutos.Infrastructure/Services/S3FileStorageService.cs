using Amazon.S3;
using Amazon.S3.Transfer;
using CatalogoDeProdutos.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CatalogoDeProdutos.Infrastructure.Services
{
    public class S3FileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _publicUrl;

        public S3FileStorageService(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS:BucketName"] ?? "products-images";
            _publicUrl = configuration["AWS:PublicURL"] ?? _s3Client.Config.ServiceURL;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName)
        {
            using var newMemoryStream = new MemoryStream();
            await file.CopyToAsync(newMemoryStream);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = fileName,
                BucketName = _bucketName,
                CannedACL = S3CannedACL.PublicRead
            };

            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);

            var serviceUrl = _publicUrl.TrimEnd('/');
            // Formato: http://127.0.0.1:9000/products-images/nome-do-arquivo
            return $"{serviceUrl}/{_bucketName}/{fileName}";
        }

        public async Task DeleteFileAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            try
            {
                // Extrai o nome do arquivo da URL
                var uri = new Uri(imageUrl);
                var fileName = Path.GetFileName(uri.LocalPath);

                await _s3Client.DeleteObjectAsync(_bucketName, fileName);
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Erro ao deletar arquivo do S3: {ex.Message}");
            }
        }
    }
}