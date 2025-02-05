using AutoMapper;
using CouponApi.Data.ValueObject;
using CouponApi.Model;

namespace CouponApi.Config
{
    public class MappingConfig
    {

        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponVO, Coupon>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
