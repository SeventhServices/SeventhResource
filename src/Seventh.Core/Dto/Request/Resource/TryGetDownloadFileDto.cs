namespace Seventh.Core.Dto.Request.Resource
{
    public class TryGetDownloadFileDto
    {
        public bool? NeedHash { get; set; }

        public int? Revision { get; set; }
    }
}