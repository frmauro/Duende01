using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using WebClient.DTO;
using WebClient.Services;
using WebClient.Services.Interfaces;

namespace WebClient.Pages.Product
{
    public class DetailsModel(IProductService productService, ICartService cartService) : PageModel
    {
        [BindProperty]
        public ProductVO Product { get; set; } = default!;
        public async Task OnGet(int id)
        {
            Product = await productService.FindProductById(id);
        }

        public async Task<IActionResult> OnPost()
        {

            CartVO cart = new()
            {
                CartHeader = new CartHeaderVO
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartDetailVO cartDetail = new()
            {
                Count = Product.Count,
                ProductId = Product.Id,
                Product = await productService.FindProductById(Product.Id)
            };


            var cartDetails = new List<CartDetailVO>
            {
                cartDetail
            };
            cart.CartDetails = cartDetails;

            var response = await cartService.AddItemToCart(cart);

            return Redirect("/Cart/Index");
        }



    }
}
