using CatalogoDeProdutos.Application.Interfaces;
using CatalogoDeProdutos.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CatalogoDeProdutos.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IProductService, ProductService>();
            return services;
        }
    }
}