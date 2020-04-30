namespace Seventh.Resource.Api.Core.Dto.Response
{
    public class AssetFileInfoDto
    {
        public string Name { get; set; }
        public int Revision { get; set; }
        public long Size { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; }
        public bool IsExist { get; set; }
    }
}
