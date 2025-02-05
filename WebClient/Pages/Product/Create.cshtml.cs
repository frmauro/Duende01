using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebClient.DTO;
using WebClient.Services.Interfaces;

namespace WebClient.Pages.Product
{
    public class CreateModel(IProductService productService) : PageModel
    {
        [BindProperty]
        public ProductVO Input { get; set; } = default!;
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            await productService.CreateProduct(Input);
            return Redirect("/Product/Index");
        }
    }
}
