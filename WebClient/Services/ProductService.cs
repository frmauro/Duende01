using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using WebClient.DTO;
using WebClient.Services.Interfaces;
using WebClient.Utils;

namespace WebClient.Services;

public class ProductService(IHttpClientFactory httpClientFactory) : IProductService
{
    private readonly HttpClient _client;
    public const string BasePath = "product";

    public string Json = string.Empty;
    public async Task<IEnumerable<ProductVO>> FindAllProducts()
    {
        var client = httpClientFactory.CreateClient("apiClient");

        //var AccessToken = await HttpContext.GetTokenAsync("access_token");

        var content = await client.GetStringAsync("https://localhost:4480/product");

        var products = JsonSerializer.Deserialize<List<ProductVO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //var response = await _client.GetAsync(BasePath);
        return products!;  //await response.ReadContentAs<List<ProductDto>>();
    }

    public async Task<ProductVO> FindProductById(long id)
    {
        var client = httpClientFactory.CreateClient("apiClient");
        var response = await client.GetAsync($"https://localhost:4480/product{id}");
        return await response.ReadContentAs<ProductVO>();
    }

    public async Task<ProductVO> CreateProduct(ProductVO model)
    {
        var client = httpClientFactory.CreateClient("apiClient");
        var response = await client.PostAsJson(BasePath, model);
        if (response.IsSuccessStatusCode) return await response.ReadContentAs<ProductVO>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<ProductVO> UpdateProduct(ProductVO model)
    {
        var response = await _client.PutAsJson(BasePath, model);
        if (response.IsSuccessStatusCode) return await response.ReadContentAs<ProductVO>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> DeleteProductById(long id)
    {
        var response = await _client.DeleteAsync($"{BasePath}/{id}");
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<bool>();
        else throw new Exception("Something went wrong when calling API");
    }
}
