using WebClient.DTO;

namespace WebClient.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductVO>> FindAllProducts();
    Task<ProductVO> FindProductById(long id);
    Task<ProductVO> CreateProduct(ProductVO model);
    Task<ProductVO> UpdateProduct(ProductVO model);
    Task<bool> DeleteProductById(long id);
}
