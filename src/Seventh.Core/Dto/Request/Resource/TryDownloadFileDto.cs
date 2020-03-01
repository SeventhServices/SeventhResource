using System.ComponentModel.DataAnnotations;

namespace Seventh.Core.Dto.Request.Resource
{
    public class TryDownloadFileDto
    {
        [Required]
        [RegularExpression("^.*\\..*$")]
        public string FileName { get; set; }

        public bool NeedHash { get; set; }

        public int? Revision { get; set; }
    }
}