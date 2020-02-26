using System.ComponentModel;
using Seventh.Resource.Common.Classes.Options;
using Seventh.Resource.Common.Utilities;
using PathOption = Seventh.Resource.Common.Classes.Options.PathOption;


namespace Seventh.Resource.Services
{
    public class ResourceLocation
    {
        public AssetSortOption SortOption { get; set; }
        public PathOption PathOption { get; set; }
        public string DownloadUrl { get; set; }
            = "https://d2kvktrbzlzxwg.cloudfront.net/revision/raw396_19dab4ea40d66c2545654a6f346d722f/";

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