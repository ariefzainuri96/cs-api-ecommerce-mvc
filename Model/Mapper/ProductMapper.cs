using AutoMapper;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;

namespace Ecommerce.Model.Mapper;

public class ProductMapper: Profile
{
    public ProductMapper()
    {
        CreateMap<ProductDto, Product>();
    }
}
