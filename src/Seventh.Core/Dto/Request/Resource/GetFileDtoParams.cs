namespace Seventh.Core.Dto.Request.Resource
{
    public class GetFileDtoParams
    {
        public bool NeedHash { get; set; }

        public int? Revision { get; set; }
    }
}