namespace Seventh.Resource.Api.Core.Dto.Response
{
    public class AssetInfoDto
    {
        public string Name { get; set; }
        public string SortedName { get; set; }
        public int Revision { get; set; }
        public string Extension { get; set; }
        public string SortedExtension { get; set; }
        public long Size { get; set; }
        public long SortedSize { get; set; }
        public string Url { get; set; }
        public string SortedUrl { get; set; }
        public bool Encrypted { get; set; }
        public bool IsExist { get; set; }
        public bool IsSortedExist { get; set; }
    }
}