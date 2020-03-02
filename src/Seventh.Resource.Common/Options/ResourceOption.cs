using Seventh.Resource.Common.Entities;

namespace Seventh.Resource.Common.Options
{
    public class ResourceOption
    {
        public Account Account { get; set; }

        public PathOption PathOption { get; set; }

        public SortOptions SortOption { get; set; }
    }
}