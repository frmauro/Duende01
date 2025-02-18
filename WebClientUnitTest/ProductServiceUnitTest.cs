using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using WebClient.DTO;
using WebClient.Services;
using WebClient.Services.Interfaces;
using Xunit;

public class ProductServiceUnitTest
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly IProductService _productService;

    public ProductServiceUnitTest()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        var client = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

        _productService = new ProductService(_httpClientFactoryMock.Object);
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
    }

    [Fact]
    public async Task FindAllProducts_ReturnsProductList()
    {
        // Arrange
        var productList = new List<ProductVO>
        {
            new ProductVO { Id = 1, Name = "Product1", Price = 10, Description = "Description1", CategoryName = "Category1", ImageURL = "URL1" },
            new ProductVO { Id = 2, Name = "Product2", Price = 20, Description = "Description2", CategoryName = "Category2", ImageURL = "URL2" }
        };
        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(productList));

        // Act
        var result = await _productService.FindAllProducts();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Product1", result.First().Name);
    }

    [Theory]
    [InlineData(1, "Product1")]
    [InlineData(2, "Product2")]
    public async Task FindProductById_ReturnsProduct(long productId, string expectedName)
    {
        // Arrange
        var product = new ProductVO { Id = productId, Name = expectedName, Price = 10, Description = "Description1", CategoryName = "Category1", ImageURL = "URL1" };
        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(product));

        // Act
        var result = await _productService.FindProductById(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal(expectedName, result.Name);
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedProduct()
    {
        // Arrange
        var product = new ProductVO { Id = 1, Name = "Product1", Price = 10, Description = "Description1", CategoryName = "Category1", ImageURL = "URL1" };
        SetupHttpResponse(HttpStatusCode.Created, JsonSerializer.Serialize(product));

        // Act
        var result = await _productService.CreateProduct(product);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Product1", result.Name);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsUpdatedProduct()
    {
        // Arrange
        var product = new ProductVO { Id = 1, Name = "Product1", Price = 10, Description = "Description1", CategoryName = "Category1", ImageURL = "URL1" };
        SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(product));

        // Act
        var result = await _productService.UpdateProduct(product);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Product1", result.Name);
    }

    [Fact]
    public async Task DeleteProductById_ReturnsTrue()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.OK, "true");

        // Act
        var result = await _productService.DeleteProductById(1);

        // Assert
        Assert.True(result);
    }
}
