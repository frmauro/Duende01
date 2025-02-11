using WebClient.DTO;

namespace WebClient.Services.Interfaces;

public interface ICartService
{
    Task<CartVO> FindCartByUserId(string? userId);
    Task<CartVO> AddItemToCart(CartVO cart);
    Task<CartVO> UpdateCart(CartVO cart);
    Task<bool> RemoveFromCart(long cartId);
    Task<bool> ApplyCoupon(CartVO cart);
    Task<bool> RemoveCoupon(string? userId);
    Task<bool> ClearCart(string userId);
    Task<object> Checkout(CartHeaderVO cartHeader);
}
