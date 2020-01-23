using SeventhServices.Resource.Common.Classes.Options;
using SeventhServices.Resource.Common.Utilities;
using PathOption = SeventhServices.Resource.Common.Classes.Options.PathOption;


namespace SeventhServices.Resource.Services
{
    public class OptionService
    {
        public AssetSortOption SortOption { get; set; }
        public PathOption PathOption { get; set; }

        public OptionService SetOptions(ResourceOption options)
        {
            PathOption = options.PathOption;
            SortOption = options.SortOption;
            CreatePath(PathOption);
            return this;
        }


        private static void CreatePath(PathOption pathOption)
        {
            CommonUtil.CreateRequireDirectories(pathOption.ConfigurePath);
            CommonUtil.CreateRequireDirectories(pathOption.IndexPath);
            CommonUtil.CreateRequireDirectories(pathOption.AssetPath);
        }
    }
}