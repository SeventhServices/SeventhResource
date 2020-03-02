using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Seventh.Core.Abstractions.Extend;
using Seventh.Core.Dto.Response.Status;
using Seventh.Core.Extend;

namespace Seventh.Core.Services
{
    public class SevenStatusService
    {
        private readonly SeventhServiceLocation _location;
        private readonly IJsonHttpExtend _httpExtend;

        public SevenStatusService(SeventhServiceLocation location,
            IJsonHttpExtend httpExtend)
        {
            _location = location;
            _httpExtend = httpExtend;
        }

        public string GetVersionInfoUrl()
        {
            return $"{_location.StatusServiceUrl}info/version";
        }

        public async Task<VersionInfoDto> TryGetVersionInfoAsync()
        {
            return await _httpExtend.TryJsonGetAsync<VersionInfoDto>(GetVersionInfoUrl());
        }
    }
}