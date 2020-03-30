using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Seventh.Core.Abstractions.Extend;

namespace Seventh.Core.Extend
{
    public class JsonHttpExtend : HttpExtend, IJsonHttpExtend
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public JsonHttpExtend(IHttpClientFactory clientFactory, JsonSerializerOptions jsonSerializerOptions)
            : base(clientFactory)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        public async Task<TResponseDto> TryJsonGetAsync<TResponseDto>(string url)
            where TResponseDto : class
        {
            var (result, response) = await TryGetAsync(url);
            if (!result)
            {
                return null;
            }
            return await DeserializeJsonContextAsync<TResponseDto>(response);
        }

        public async Task<TResponseDto> TryJsonGetAsync<TResponseDto>(string url, IEnumerable<KeyValuePair<string, string>> querys)
            where TResponseDto : class
        {
            var queryString = string.Empty;
            if (querys != null)
            {
                var queryArray = querys.ToArray();
                queryString = string.Join("&",
                    queryArray.Select(q =>
                       string.Concat(q.Key, "=", q.Value)));
            }
            var (result, response) = await TryGetAsync(string.Concat(url, "?", queryString));
            if (!result)
            {
                return null;
            }
            return await DeserializeJsonContextAsync<TResponseDto>(response);
        }

        public async Task<TResponseDto> TryJsonPostAsync<TResponseDto>(string url)
            where TResponseDto : class
        {
            var (result, response) = await TryPostAsync(url);
            if (!result)
            {
                return null;
            }
            return await DeserializeJsonContextAsync<TResponseDto>(response);
        }

        public async Task<TResponseDto> TryJsonPostAsync<TRequestDto, TResponseDto>(string url, TRequestDto body)
            where TRequestDto : class
            where TResponseDto : class
        {
            var jsonContext = new JsonContext(JsonSerializer.Serialize(body, _jsonSerializerOptions));
            var (result, response) = await TryPostAsync(url, jsonContext);
            if (!result)
            {
                return null;
            }
            return await DeserializeJsonContextAsync<TResponseDto>(response);
        }

        private async Task<TResponseDto> DeserializeJsonContextAsync<TResponseDto>(HttpResponseMessage response)
            where TResponseDto : class
        {
            return await JsonSerializer.DeserializeAsync<TResponseDto>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
        }
    }
}