using System;
using System.Net.Http;

namespace Seventh.Resource.Services
{
    public class DownloadClient
    {
        public HttpClient Client { get; }

        public DownloadClient(HttpClient httpClient, ResourceLocation option)
        {
            httpClient.BaseAddress = new Uri(option.DownloadUrl);
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            Client = httpClient;
        }
    }
}