using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers;

public class ProductController : BaseController<ProductController>
{
    private readonly IProductService _productService;

    public ProductController(ILogger<ProductController> logger, IProductService productService) : base(logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    public async Task<IActionResult> ProductIndex()
    {
        var products = await _productService.FindAllProducts("");
        return View(products);
    }

    public async Task<IActionResult> ProductCreate()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ProductCreate(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _productService.CreateProduct(model, await GetToken());
            if (response != null) return RedirectToAction(nameof(ProductIndex));
        }
        return View(model);
    }

    //UPDATE
    public async Task<IActionResult> ProductUpdate(long id)
    {
        var model = await _productService.FindProductById(id, await GetToken());
        if (model != null) return View(model);
        return NotFound();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ProductUpdate(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _productService.UpdateProduct(model, await GetToken());
            if (response != null) return RedirectToAction(nameof(ProductIndex));
        }
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> ProductDelete(long id)
    {
        var model = await _productService.FindProductById(id, await GetToken());
        if (model != null) return View(model);
        return NotFound();
    }

    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    public async Task<IActionResult> ProductDelete(ProductViewModel model)
    {
        var response = await _productService.DeleteProductById(model.Id, await GetToken());
        if (response) return RedirectToAction(nameof(ProductIndex));
        return View(model);
    }

}
