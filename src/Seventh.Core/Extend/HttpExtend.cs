using System;
using System.Net.Http;
using System.Threading.Tasks;
using Seventh.Core.Abstractions.Extend;

namespace Seventh.Core.Extend
{
    public class HttpExtend : IHttpExtend
    {
        private readonly HttpClient _client;
        public Action<HttpRequestMessage> BeforeSendRequest { get; set; }

        public HttpExtend(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
        }

        public async Task<string> TryStringGetAsync(string url)
        {
            var (result, response) = await TryGetAsync(url);
            if (!result)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> TryStringPostAsync(string url)
        {
            var (result, response) = await TryPostAsync(url);
            if (!result)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> TryStringPostAsync(string url, HttpContent content)
        {
            var (result, response) = await TryPostAsync(url, content);
            if (!result)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<(bool, HttpResponseMessage)> TryGetAsync(string url)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            return await TrySendAsync(request);
        }

        public async Task<(bool, HttpResponseMessage)> TryPostAsync(string url)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post
            };
            return await TrySendAsync(request);
        }

        public async Task<(bool, HttpResponseMessage)> TryPostAsync(string url, HttpContent content)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = content
            };
            return await TrySendAsync(request);
        }

        public async Task<(bool, HttpResponseMessage)> TrySendAsync(HttpRequestMessage request)
        {
            BeforeSendRequest?.Invoke(request);
            var response = await _client.SendAsync(request);
            return (response.IsSuccessStatusCode, response);
        }

    }
}