using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Chess_Up.Controllers
{
    public class ProxyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProxyController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Fetch(string url)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, response.Content.Headers.ContentType.MediaType);
        }
    }
}
