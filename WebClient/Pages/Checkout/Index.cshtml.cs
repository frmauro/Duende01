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
        public CartVO? CartVo { get; set; }

        public async Task OnGet()
        {
            var usarId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var cart = await cartService.FindCartByUserId(usarId);

            if (cart.CartHeader != null)
            {
                foreach (var detail in cart.CartDetails)
                {
                    cart.CartHeader.PurshaseAmount += (detail.Product.Price * detail.Count);
                }
                cart.CartHeader.PurshaseAmount -= cart.CartHeader.DiscountTotal;
            }

            CartVo = cart;
        }


        public async Task<IActionResult> OnPost()
        {
            var usarId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var cart = await cartService.Checkout(CartVo?.CartHeader!);

            if (cart != null)
            {
                return Redirect("/Cart/Confirmation");
            }

            return Page();
        }
    }
}
