using AutoMapper;
using CatalogoDeProdutos.Application.DTOs;
using CatalogoDeProdutos.Domain.Entities;

namespace CatalogoDeProdutos.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())); 
        }
    }
}