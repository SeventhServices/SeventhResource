using System.Net.Http;
using System.Threading.Tasks;

namespace Seventh.Core.Abstractions.Extend
{
    public interface IHttpExtend
    {
        public Task<string> TryStringGetAsync(string url);
        public Task<string> TryStringPostAsync(string url);
        public Task<string> TryStringPostAsync(string url, HttpContent content);
        public Task<(bool, HttpResponseMessage)> TryGetAsync(string url);
        public Task<(bool, HttpResponseMessage)> TryPostAsync(string url);
        public Task<(bool, HttpResponseMessage)> TryPostAsync(string url, HttpContent content);
        public Task<(bool, HttpResponseMessage)> TrySendAsync(HttpRequestMessage request);
    }
}