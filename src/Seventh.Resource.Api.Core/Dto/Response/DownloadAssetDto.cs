namespace Seventh.Resource.Api.Core.Dto.Response
{
    public class DownloadAssetDto
    {
        public bool CanFound { get; set; }
        public bool DownloadCompleted { get; set; }
        public string DownloadFileName { get; set; }
        public AssetInfoDto FileInfo { get; set; }
    }
}