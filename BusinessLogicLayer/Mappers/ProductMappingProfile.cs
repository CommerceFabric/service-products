using AutoMapper;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Mappers
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductAddRequest, Product>();
            CreateMap<ProductUpdateRequest, Product>();
            CreateMap<Product, ProductResponse>();
        }
    }
}
