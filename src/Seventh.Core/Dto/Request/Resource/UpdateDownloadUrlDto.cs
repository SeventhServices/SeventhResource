using System.ComponentModel.DataAnnotations;

namespace Seventh.Core.Dto.Request.Resource
{

    public class UpdateDownloadUrlDto
    {
        [Required]
        public string Url { get; set; }
    }
}