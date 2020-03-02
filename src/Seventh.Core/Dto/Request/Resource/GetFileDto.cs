using System.ComponentModel.DataAnnotations;

namespace Seventh.Core.Dto.Request.Resource
{
    public class GetFileDto : GetFileDtoParams
    {
        [Required]
        [RegularExpression("^.*\\..*$")]
        public string FileName { get; set; }
    }
}