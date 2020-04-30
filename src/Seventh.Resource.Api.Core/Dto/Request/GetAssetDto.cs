using System.ComponentModel.DataAnnotations;

namespace Seventh.Resource.Api.Core.Dto.Request
{
    public class GetAssetDto : GetAssetParamsDto
    {
        [Required]
        [RegularExpression("^.*\\..*$")]
        public string FileName { get; set; }
    }
}