using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebClient.DTO;
using WebClient.Services.Interfaces;

namespace WebClient.Pages.Cart
{
    public class IndexModel(IProductService productService, ICartService cartService, ICouponService couponService) : PageModel
    {
        [BindProperty]
        public CartVO CartVo { get; set; }
        public async Task OnGet()
        {
            var usarId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var cart = await cartService.FindCartByUserId(usarId);

            if (cart.CartHeader != null)
            {
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    var couponVO = await couponService.FindCouponByCode(cart.CartHeader.CouponCode);
                    if (couponVO?.CouponCode != null)
                    {
                        cart.CartHeader.DiscountTotal += couponVO.DiscountAmount;
                    }
                }
                foreach (var detail in cart.CartDetails)
                {
                    cart.CartHeader.PurshaseAmount += (detail.Product.Price * detail.Count);
                }
                cart.CartHeader.PurshaseAmount -= cart.CartHeader.DiscountTotal;
            }

            CartVo = cart;
        }


        public async Task<IActionResult> OnPostApplyCouponAsync()
        {
            await cartService.ApplyCoupon(CartVo);
            return Redirect("/Cart/Index");
        }

        public async Task<IActionResult> OnPostRemoveCouponAsync()
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            await cartService.RemoveCoupon(userId);
            return Redirect("/Cart/Index");
        }



    }
}
