using System;

namespace Seventh.Core.Dto.Status
{
    public class AssetVersionDto
    {
        public int Revision { get; set; }
        public string SubDomain { get; set; }
        public string DownloadUrl { get; set; }
        public DateTimeOffset LastModify { get; set; }
    }
}
