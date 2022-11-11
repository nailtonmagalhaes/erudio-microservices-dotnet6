using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services;

public class ProductService : BaseService, IProductService
{
    public ProductService(HttpClient client) : base(client, "api/v1/product") { }

    public async Task<IEnumerable<ProductViewModel>> FindAllProducts(string token)
    {
        SetHeaderToken(token);
        var response = await _client.GetAsync(_basePath);
        return await response.ReadContentAs<List<ProductViewModel>>();
    }

    public async Task<ProductViewModel> FindProductById(long id, string token)
    {
        SetHeaderToken(token);
        var response = await _client.GetAsync($"{_basePath}/{id}");
        return await response.ReadContentAs<ProductViewModel>();
    }

    public async Task<ProductViewModel> CreateProduct(ProductViewModel model, string token)
    {
        SetHeaderToken(token);
        var response = await _client.PostAsJson(_basePath, model);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<ProductViewModel>();
        throw new Exception("Somethig went wrong when calling API");
    }

    public async Task<ProductViewModel> UpdateProduct(ProductViewModel model, string token)
    {
        SetHeaderToken(token);
        var response = await _client.PutAsJson(_basePath, model);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<ProductViewModel>();
        throw new Exception("Somethig went wrong when calling API");
    }

    public async Task<bool> DeleteProductById(long id, string token)
    {
        SetHeaderToken(token);
        var response = await _client.DeleteAsync($"{_basePath}/{id}");
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<bool>();
        throw new Exception("Somethig went wrong when calling API");
    }
}
