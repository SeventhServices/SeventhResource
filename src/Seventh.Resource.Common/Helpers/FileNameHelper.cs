using System.IO;
using System.Linq;

namespace Seventh.Resource.Common.Helpers
{
    public static class FileNameHelper
    {
        public static int ParseRev(string fileName)
        {
            var revNamePart = Path.GetFileNameWithoutExtension(fileName).Split('_').LastOrDefault();
            if (revNamePart == null)
            {
                return 0;
            }

            if (revNamePart.StartsWith('r'))
            {
                revNamePart = revNamePart.TrimStart('r');
                return int.TryParse(revNamePart, out int revision) ? revision : 0;
            }

            return 0;
        }
    }
}
