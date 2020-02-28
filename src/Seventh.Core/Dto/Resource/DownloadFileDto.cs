namespace Seventh.Core.Dto.Resource
{
    public class GetOrDownloadFileDto
    {
        public bool? NeedHash { get; set; }

        public int? Revision { get; set; }
    }
}