using System.Collections.Generic;

namespace Seventh.Core.Dto.Status
{
    public class AssetModifyDto
    {
        public int Revision { get; set; }
        public int FromRevision { get; set; }
        public long TotalSize { get; set; }
        public IEnumerable<string> DeletedFiles { get; set; }
        public IEnumerable<string> ModifyFiles { get; set; }
        public IEnumerable<string> OneByOneModifyFiles { get; set; }
    }
}
