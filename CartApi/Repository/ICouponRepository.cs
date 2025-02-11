
using CartApi.Data.ValueObject;

namespace CartApi.Repository;

public interface ICouponRepository
{
    Task<CouponVO> GetCoupon(string couponCode);
}
