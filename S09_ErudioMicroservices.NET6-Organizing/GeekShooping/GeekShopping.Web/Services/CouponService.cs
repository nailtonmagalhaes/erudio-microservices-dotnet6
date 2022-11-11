using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using System.Net;

namespace GeekShopping.Web.Services
{
    public class CouponService : BaseService, ICouponService
    {
        public CouponService(HttpClient client) : base(client, "api/v1/coupon") { }

        public async Task<CouponViewModel> GetCoupon(string code, string token)
        {
            SetHeaderToken(token);
            var response = await _client.GetAsync($"{_basePath}/{code}");
            if (response.StatusCode != HttpStatusCode.OK) return new CouponViewModel();
            return await response.ReadContentAs<CouponViewModel>();
        }
    }
}
