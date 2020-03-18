namespace Seventh.Core.Dto.Response.Resource
{
    public class DownloadAssetDto
    {
        public bool CanFound { get; set; }
        public bool DownloadCompleted { get; set; }
        public string DownloadFileName { get; set; }
        public AssetInfoDto FileInfo {get; set; }
    }
}