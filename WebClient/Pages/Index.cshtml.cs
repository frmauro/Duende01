using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebClient.DTO;
using WebClient.Services.Interfaces;

namespace WebClient.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IProductService _productService;

    [BindProperty]
    public IEnumerable<ProductVO> Products { get; set; } = default!;

    public IndexModel(ILogger<IndexModel> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    public async Task OnGet()
    {
        Products = await _productService.FindAllProducts();
    }
}
