using System.ComponentModel.DataAnnotations;

namespace Seventh.Resource.Api.Dto
{
    public class UpdateDownloadUrlDto
    {
        [Required]
        public string DownloadUrl { get; set; }
    }
}