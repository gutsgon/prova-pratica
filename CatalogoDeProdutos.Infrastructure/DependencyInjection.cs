using Amazon.S3;
using CatalogoDeProdutos.Application.Interfaces;
using CatalogoDeProdutos.Domain.Interfaces;
using CatalogoDeProdutos.Infrastructure.Context;
using CatalogoDeProdutos.Infrastructure.Repositories;
using CatalogoDeProdutos.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogoDeProdutos.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Configuração do EF Core (Banco)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // 2. Repositórios
            services.AddScoped<IProductRepository, ProductRepository>();

            // 3. Configuração AWS S3 / MinIO
            var awsOptions = configuration.GetAWSOptions();
            
            // Força o uso da URL do MinIO definida no appsettings
            awsOptions.Credentials = new Amazon.Runtime.BasicAWSCredentials(
                configuration["AWS:AccessKey"], 
                configuration["AWS:SecretKey"]);
            
            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonS3>();

            // Registra o Serviço de Upload
            services.AddScoped<IFileStorageService, S3FileStorageService>();

            return services;
        }
    }
}