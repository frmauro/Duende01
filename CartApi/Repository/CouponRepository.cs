using CartApi.Data.ValueObject;
using CartApi.Utils;

namespace CartApi.Repository;

public class CouponRepository(IHttpClientFactory httpClientFactory) : ICouponRepository
{
    public async Task<CouponVO> GetCoupon(string couponCode)
    {
        var _client = httpClientFactory.CreateClient("couponApi");
        var response = await _client.GetAsync($"https://localhost:4450/find-coupon/{couponCode}");
        if (response.StatusCode != System.Net.HttpStatusCode.OK) return new CouponVO();
        return await response.ReadContentAs<CouponVO>();
    }

}
