using System.ComponentModel;
using Seventh.Resource.Common.Options;
using Seventh.Resource.Common.Utilities;
using PathOption = Seventh.Resource.Common.Options.PathOption;


namespace Seventh.Resource.Services
{
    public class ResourceLocation
    {
        public AssetSortOption SortOption { get; set; }
        public PathOption PathOption { get; set; }
        public string DownloadUrl { get; set; }

        public ResourceLocation SetOptions(ResourceOption options)
        {
            PathOption = options.PathOption;
            SortOption = options.SortOption;
            CreatePath(PathOption);
            return this;
        }

        private static void CreatePath(PathOption pathOption)
        {
            CommonUtil.CreateRequireDirectories(pathOption.IndexPath);
            CommonUtil.CreateRequireDirectories(pathOption.AssetPath);
        }
    }
}