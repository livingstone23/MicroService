using AutoMapper;
using Manager.Services.CouponAPI.Models;
using Manager.Services.CouponAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {

                config.CreateMap<CouponDto, Coupon>().ReverseMap();

            });

            return mappingConfig;
        }
    }
}
