

namespace Seventh.Core.Dto.Response.Resource
{
    public class GetFileResultDto
    {
        public bool IsFileExist { get; set; }
        public bool IsRealFileExist { get; set; }
        public AssetFileInfoDto FileInfo {get; set; }
    }
}