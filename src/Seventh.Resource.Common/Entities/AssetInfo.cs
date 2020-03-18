using System;
using System.Collections.Generic;
using System.Text;

namespace Seventh.Resource.Common.Entities
{
    public class AssetInfo
    {
        public AssetFileInfo MirrorFileInfo { get; set; }
        public AssetFileInfo SortedFileInfo { get; set; }

        public AssetInfo SetRevision(int revision)
        {
            MirrorFileInfo.Revision = revision;
            SortedFileInfo.Revision = revision;
            return this;
        }
    }
}
