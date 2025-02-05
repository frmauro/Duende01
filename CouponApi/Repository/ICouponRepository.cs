using CouponApi.Data.ValueObject;

namespace CouponApi.Repository;

public interface ICouponRepository
{
    Task<CouponVO> GetCouponByCouponCode(string couponCode);
}
