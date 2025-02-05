using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using WebClient.DTO;
using WebClient.Services.Interfaces;

namespace WebClient.Pages.Product
{
    public class IndexModel(IProductService productService) : PageModel
    {
        public string Json = string.Empty;
        public IList<ProductVO> Products { get; set; }

        public async Task OnGet()
        {
            var products = await productService.FindAllProducts();
            Products = products.ToList();
        }
    }
}
