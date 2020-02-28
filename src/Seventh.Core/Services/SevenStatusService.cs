using Seventh.Core.Dto.Status;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Seventh.Core.Services
{
    public class SevenStatusService
    {
        private readonly SeventhServiceLocation _location;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public SevenStatusService(SeventhServiceLocation location,
            IHttpClientFactory clientFactory)
        {
            _location = location;
            _client = clientFactory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public string GetVersionInfoUrl()
        {
            return $"{_location.StatusServiceUrl}info/version";
        }

        public async Task<VersionInfoDto> TryGetVersionInfoAsync()
        {
            var response = await _client.GetAsync(GetVersionInfoUrl());
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var json =  await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<VersionInfoDto>(json,_jsonOptions);
        }
    }
}