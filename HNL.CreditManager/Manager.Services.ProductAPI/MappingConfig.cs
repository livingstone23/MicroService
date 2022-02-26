using AutoMapper;
using Manager.Services.ProductAPI.Models;
using Manager.Services.ProductAPI.Models.Dto;

namespace Manager.Services.ProductAPI
{

    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>();
                config.CreateMap<Product, ProductDto>();
            });

            return mappingConfig;
        }


    }
}
