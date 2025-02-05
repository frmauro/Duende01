using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebClient.Services.Interfaces;

namespace WebClient.Pages.Cart
{
    public class RemoveModel(ICartService cartService) : PageModel
    {
        public async Task<IActionResult> OnGet(int id)
        {
            var usarId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var isRemoved = await cartService.RemoveFromCart(id);
            return RedirectToPage("/Cart/Index");
        }
    }
}
