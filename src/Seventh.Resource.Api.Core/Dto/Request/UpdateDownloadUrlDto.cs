using System.ComponentModel.DataAnnotations;

namespace Seventh.Resource.Api.Core.Dto.Request
{

    public class UpdateDownloadUrlDto
    {
        [Required]
        public string Url { get; set; }
    }
}