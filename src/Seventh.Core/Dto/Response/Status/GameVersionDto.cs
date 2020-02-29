using System;

namespace Seventh.Core.Dto.Response.Status
{
    public class GameVersionDto
    {
        public string Version { get; set; }
        public DateTimeOffset LastModify { get; set; }
    }
}
