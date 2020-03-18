using System.ComponentModel.DataAnnotations;

namespace Seventh.Core.Dto.Request.Resource
{
    public class GetAssetDto : GetAssetParamsDto
    {
        [Required]
        [RegularExpression("^.*\\..*$")]
        public string FileName { get; set; }
    }
}