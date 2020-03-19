using Seventh.Core.Dto.Response.Resource;
using Seventh.Core.Dto.Response.Status;
using System;
using System.Net.Http;
using System.Reflection;
using Seventh.Core.Abstractions.Extend;
using System.Collections.Generic;
using System.Threading.Tasks;
using Seventh.Core.Extend;
using Seventh.Core.Dto.Request.Resource;

namespace Seventh.Core.Services
{
    public class SevenResourceService
    {
        private readonly IJsonHttpExtend _httpExtend;
        public string BaseUrl { get; }

        public SevenResourceService(SeventhServiceLocation location,
            IJsonHttpExtend httpExtend)
        {
            _httpExtend = httpExtend;
            BaseUrl = location.ResourceServiceUrl;
        }

        public async Task<DownloadAssetDto> TryDownloadNewFileAsync(string fileName,int revision, bool needHash = false)
        {
            var queries = new []
            {
                new KeyValuePair<string, string>(nameof(revision),revision.ToString()),
                new KeyValuePair<string, string>(nameof(needHash),needHash.ToString()),
            }; 
            return await _httpExtend.TryJsonGetAsync<DownloadAssetDto>(string.Concat(BaseUrl,"asset/download/",fileName),queries);
        }
        
        public async Task<DownloadAssetDto> TryDownloadLargeCardAsync(int cardId)
        {
            return await _httpExtend.TryJsonPostAsync<DownloadAssetDto>(string.Concat(BaseUrl,"asset/download/",cardId.ToString()) );
        }

        public async Task<IEnumerable<DownloadAssetDto>> TryDownloadNewFilesAsync(IEnumerable<GetAssetDto> dtoList)
        {
            return await _httpExtend.TryJsonPostAsync<IEnumerable<GetAssetDto>,IEnumerable<DownloadAssetDto>>(string.Concat(BaseUrl,"asset/download/"),dtoList);
        }

        public async Task<RefreshedDownloadUrlDto> UpdateDownloadUrl()
        {
            return await _httpExtend.TryJsonPostAsync<RefreshedDownloadUrlDto>(string.Concat(BaseUrl,"config/downloadUrl"));
        }
    }
}