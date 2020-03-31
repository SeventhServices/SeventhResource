using System.Threading.Tasks;
using Seventh.Core.Abstractions.Extend;
using Seventh.Core.Dto.Response.Status;

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

        public string GetBasicModifyUrl()
        {
            return $"{_location.StatusServiceUrl}asset/basic/modify";
        }

        public async Task<VersionInfoDto> TryGetVersionInfoAsync()
        {
            return await _httpExtend.TryJsonGetAsync<VersionInfoDto>(GetVersionInfoUrl());
        }

        public async Task<AssetModifyDto> TryGetBasicModifyAsync()
        {
            return await _httpExtend.TryJsonGetAsync<AssetModifyDto>(GetBasicModifyUrl());
        }
    }
}