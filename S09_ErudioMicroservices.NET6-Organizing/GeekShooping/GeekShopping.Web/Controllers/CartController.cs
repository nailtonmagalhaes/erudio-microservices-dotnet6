using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers;

public class CartController : BaseController<CartController>
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;

    public CartController(ILogger<CartController> logger,
                          IProductService productService,
                          ICartService cartService,
                          ICouponService couponService) : base(logger)
    {
        _productService = productService;
        _cartService = cartService;
        _couponService = couponService;
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        return View(await FindUserCart());
    }

    [HttpPost]
    [ActionName("ApplyCoupon")]
    public async Task<IActionResult> ApplyCoupon(CartViewModel model)
    {
        var response = await _cartService.ApplyCoupon(model, await GetToken());

        if (response)
            return RedirectToAction(nameof(CartIndex));
        return View();
    }

    [HttpPost]
    [ActionName("RemoveCoupon")]
    public async Task<IActionResult> RemoveCoupon()
    {
        var response = await _cartService.RemoveCoupon(UserId, await GetToken());

        if (response)
            return RedirectToAction(nameof(CartIndex));
        return View();
    }

    public async Task<IActionResult> Remove(int id)
    {
        var response = await _cartService.RemoveFromCart(id, await GetToken());

        if (response)
            return RedirectToAction(nameof(CartIndex));

        return View();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        return View(await FindUserCart());
    }

    private async Task<CartViewModel> FindUserCart()
    {
        var token = await GetToken();
        var response = await _cartService.FindCartByUserId(UserId, token);
        if (response?.CartHeader != null)
        {
            if (!string.IsNullOrEmpty(response.CartHeader.CouponCode))
            {
                var coupon = await _couponService.GetCoupon(response.CartHeader.CouponCode, token);
                if (coupon?.CouponCode != null)
                    response.CartHeader.DiscountAmount = coupon.DiscountAmount;
            }
            foreach (var detail in response.CartDetails)
                response.CartHeader.PurchaseAmount += (detail.Product.Price * detail.Count);

            response.CartHeader.PurchaseAmount -= response.CartHeader.DiscountAmount;
        }

        return response;
    }
}
