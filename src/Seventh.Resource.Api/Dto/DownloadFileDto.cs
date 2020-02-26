using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Seventh.Resource.Api.Dto
{
    public class GetOrDownloadFileDto
    {
        public bool? NeedHash { get; set; }

        public int? Revision { get; set; }
    }
}