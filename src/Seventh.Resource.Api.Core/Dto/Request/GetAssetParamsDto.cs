namespace Seventh.Resource.Api.Core.Dto.Request
{
    public class GetAssetParamsDto
    {
        public bool NeedHash { get; set; }
        public int? Revision { get; set; }
    }
}