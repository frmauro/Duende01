using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebClient.DTO;
using WebClient.Services;
using WebClient.Services.Interfaces;

namespace WebClient.Pages.Checkout
{
    public class IndexModel(ICartService cartService) : PageModel
    {
        [BindProperty]
        public CartVO? CartVo { get; set; } = new CartVO();

        [BindProperty]
        public string? ErrorMessage { get; set; }

        public async Task OnGet()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == "sub")?.Value;
            if (userId != null)
            {
                CartVo = await GetCartVo(userId);
            }
        }


        public async Task<IActionResult> OnPost()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == "sub")?.Value;

            if (userId == null)
            {
                ErrorMessage = "User not found. Please login again.";
                return Page();
            }
            try
            {
                var response = await cartService.Checkout(CartVo?.CartHeader!);

                if (response != null && response.GetType() == typeof(string))
                {
                    ErrorMessage = response!.ToString();
                    var cart = await GetCartVo(userId);
                    CartVo = cart;
                    return Page();
                }
                else if (response != null)
                {
                    return Redirect("/Cart/Confirmation");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            return Page();
        }



        private async Task<CartVO> GetCartVo(string userId)
        {
            var cart = await cartService.FindCartByUserId(userId);

            if (cart.CartHeader != null)
            {
                foreach (var detail in cart.CartDetails)
                {
                    cart.CartHeader.PurshaseAmount += (detail.Product.Price * detail.Count);
                }
                cart.CartHeader.PurshaseAmount -= cart.CartHeader.DiscountTotal;
            }

            return cart;
        }
    }
}
