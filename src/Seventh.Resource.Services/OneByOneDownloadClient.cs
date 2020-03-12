using System;
using System.Net.Http;

namespace Seventh.Resource.Services
{
    public class OneByOneDownloadClient
    {
        public HttpClient Client { get; }

        public OneByOneDownloadClient(HttpClient httpClient, ResourceLocation option)
        {
            httpClient.BaseAddress = new Uri(option.DownloadBaseUrl);
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            Client = httpClient;
        }
    }
}