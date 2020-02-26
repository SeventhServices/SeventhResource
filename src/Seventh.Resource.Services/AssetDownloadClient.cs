using System;
using System.Net.Http;

namespace Seventh.Resource.Services
{
    public class AssetDownloadClient
    {
        public HttpClient Client { get; }

        public AssetDownloadClient(HttpClient httpClient, ResourceLocation option)
        {
            httpClient.BaseAddress = new Uri(option.DownloadUrl);
            Client = httpClient;
        }
    }
}