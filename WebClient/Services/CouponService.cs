using IdentityModel.Client;
using System.Net.Http.Headers;
using System.Reflection;
using WebClient.DTO;
using WebClient.Services.Interfaces;
using WebClient.Utils;

namespace WebClient.Services;

public class CouponService(IHttpClientFactory httpClientFactory) : ICouponService
{
    //private readonly HttpClient _client;
    public const string BasePath = "cart";

    public async Task<CouponVO> FindCouponByCode(string code)
    {
        var _client = httpClientFactory.CreateClient("cartApi");
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync($"https://localhost:4450/find-coupon/{code}");
        return await response.ReadContentAs<CouponVO>();
    }
}
