using System.ComponentModel.DataAnnotations;

namespace Seventh.Core.Dto.Resource
{
    public class UpdateDownloadUrlDto
    {
        [Required]
        public string DownloadUrl { get; set; }
    }
}