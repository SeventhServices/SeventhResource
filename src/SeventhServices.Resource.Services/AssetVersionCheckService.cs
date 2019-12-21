using System;
using System.Threading.Tasks;
using SeventhServices.Client.Network.Interfaces;
using SeventhServices.Client.Network.Models.Request;
using SeventhServices.Resource.Services.Abstractions;

namespace SeventhServices.Resource.Services
{
    public class AssetVersionCheckService : ICheckService
    {
        private readonly StatusService _statusService;
        private readonly ICheckUpdateApiClient _apiClient;

        public AssetVersionCheckService(StatusService statusService, ICheckUpdateApiClient apiClient)
        {
            _statusService = statusService;
            _apiClient = apiClient;
        }

        public int NowRev { get; set; }

        public void NoticeStatus()
        {
            _statusService.Rev = NowRev;
        }

        public async Task<int> TryUpdate()
        {
            var inspection = await _apiClient.Inspection(new InspectionRequest());
            if (inspection.IsDevelopment)
            {
                throw new Exception("Rev check failed");
            }

            NowRev = inspection.Rev;
            NoticeStatus();
            return NowRev;
        }
    }
}