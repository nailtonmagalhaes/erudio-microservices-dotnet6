using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services;

public class CartService : BaseService, ICartService
{
    public CartService(HttpClient client) : base(client, "api/v1/cart") { }

    public async Task<CartViewModel> FindCartByUserId(string userId, string token)
    {
        SetHeaderToken(token);
        var response = await _client.GetAsync($"{_basePath}/find-cart/{userId}");
        return await response.ReadContentAs<CartViewModel>();
    }

    public async Task<CartViewModel> AddItemToCart(CartViewModel model, string token)
    {
        SetHeaderToken(token);
        var response = await _client.PostAsJson($"{_basePath}/add-cart", model);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<CartViewModel>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<CartViewModel> UpdateCart(CartViewModel model, string token)
    {
        SetHeaderToken(token);
        var response = await _client.PutAsJson($"{_basePath}/update-cart", model);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<CartViewModel>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> RemoveFromCart(long cartId, string token)
    {
        SetHeaderToken(token);
        var response = await _client.DeleteAsync($"{_basePath}/remove-cart/{cartId}");
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<bool>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> ApplyCoupon(CartViewModel model, string token)
    {
        SetHeaderToken(token);
        var response = await _client.PostAsJson($"{_basePath}/apply-coupon", model);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<bool>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> RemoveCoupon(string userId, string token)
    {
        SetHeaderToken(token);
        var response = await _client.DeleteAsync($"{_basePath}/remove-coupon/{userId}");
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<bool>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<object> Checkout(CartHeaderViewModel model, string token)
    {
        SetHeaderToken(token);
        var response = await _client.PostAsJson($"{_basePath}/checkout", model);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<CartHeaderViewModel>();
        else if (response.StatusCode.ToString().Equals("PreconditionFailed"))
            return "Coupon Price has changed, please confirm!";
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> ClearCart(string userId, string token)
    {
        throw new NotImplementedException();
    }

}
