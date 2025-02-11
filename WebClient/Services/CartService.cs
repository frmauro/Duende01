using WebClient.DTO;
using WebClient.Services.Interfaces;
using WebClient.Utils;

namespace WebClient.Services;

public class CartService(IHttpClientFactory httpClientFactory) : ICartService
{
    //private readonly HttpClient _client;
    public const string BasePath = "cart";

    public async Task<CartVO> FindCartByUserId(string? userId)
    {
        var _client = httpClientFactory.CreateClient("cartApi");
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync($"https://localhost:7229/find-cart/{userId}");
        return await response.ReadContentAs<CartVO>();
    }

    public async Task<CartVO> AddItemToCart(CartVO model)
    {
        var _client = httpClientFactory.CreateClient("cartApi");
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJson("https://localhost:7229/add-cart", model);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<CartVO>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<CartVO> UpdateCart(CartVO model)
    {
        var _client = httpClientFactory.CreateClient("cartApi");
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PutAsJson($"{BasePath}/update-cart", model);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<CartVO>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> RemoveFromCart(long cartId)
    {
        var _client = httpClientFactory.CreateClient("cartApi");
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.DeleteAsync($"https://localhost:7229/remove-cart/{cartId}");
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<bool>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> ApplyCoupon(CartVO cart)
    {
        var _client = httpClientFactory.CreateClient("cartApi");
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJson("https://localhost:7229/apply-coupon", cart);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<bool>();
        else throw new Exception("Something went wrong when calling API");

    }


    public async Task<bool> RemoveCoupon(string? userId)
    {
        var _client = httpClientFactory.CreateClient("cartApi");
        var response = await _client.DeleteAsync($"https://localhost:7229/remove-coupon/{userId}");
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<bool>();
        else throw new Exception("Something went wrong when calling API");

    }

    public async Task<object> Checkout(CartHeaderVO cartHeader)
    {
        var _client = httpClientFactory.CreateClient("cartApi");
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PostAsJson("https://localhost:7229/checkout", cartHeader);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<CartVO>();
        else if (response.StatusCode.ToString().Equals("PreconditionFailed"))
        {
            return "Coupon price has changed, please confirm! ";
        }
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> ClearCart(string userId)
    {
        throw new NotImplementedException();
    }
}
