namespace Seventh.Core.Dto.Response.Resource
{
    public class DownloadFileDto
    {
        public bool Result { get; set; }
        public string FileName { get; set; }
        public string RealFileName { get; set; }
        public int Revision { get; set; }
        public string MirrorUrl { get; set; }
        public string SortedUrl { get; set; }
    }
}