using WebClient.DTO;

namespace WebClient.Services.Interfaces;

public interface ICouponService
{
    Task<CouponVO> FindCouponByCode(string code);
}
