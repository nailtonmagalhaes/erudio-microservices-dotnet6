using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class BaseController<T> : Controller
    {
        protected readonly ILogger<T> _logger;

        public BaseController(ILogger<T> logger)
        {
            _logger = logger;
        }

        protected async Task<string> GetToken() => await HttpContext.GetTokenAsync("access_token");

        protected string UserId => User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
    }
}
