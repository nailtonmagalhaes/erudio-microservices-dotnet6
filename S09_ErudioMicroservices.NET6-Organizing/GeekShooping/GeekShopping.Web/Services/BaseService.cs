using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public abstract class BaseService
    {
        protected readonly HttpClient _client;
        protected readonly string _basePath;

        public BaseService(HttpClient client, string basePath)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        }

        protected void SetHeaderToken(string token) =>
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
