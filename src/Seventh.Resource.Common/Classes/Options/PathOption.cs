using System.IO;

namespace Seventh.Resource.Common.Classes.Options
{
    public class PathOption
    {
        public string RootPath { get; set; }
        public AssetPath AssetPath { get; set; }
        public IndexPath IndexPath { get; set; }
        public ConfigurePath ConfigurePath { get; set; }

        public PathOption(string rootPath)
        {
            RootPath = rootPath;
            //ConfigurePath = new ConfigurePath
            //{
            //    RootPath = ConfigureWatcher.SavePath
            //};
            IndexPath = new IndexPath
            {
                RootPath = Path.Combine(RootPath, "Index")
            };
            AssetPath = new AssetPath
            {
                RootPath = Path.Combine(RootPath, "Asset"),
                DownloadTempRootPath = Path.Combine(RootPath, "Temp"),
            };
            AssetPath.SortedAssetPath = Path.Combine(AssetPath.RootPath, "Sorted");
            AssetPath.MirrorAssetRootPath = Path.Combine(AssetPath.RootPath, "Mirror");
            AssetPath.RevMirrorAssetPath = Path.Combine(AssetPath.MirrorAssetRootPath, "Revs");
            AssetPath.GameMirrorAssetPath = Path.Combine(AssetPath.MirrorAssetRootPath, "Game");
            AssetPath.ApkDownloadTempPath = Path.Combine(AssetPath.DownloadTempRootPath, "Apk");
            AssetPath.AssetDownloadTempPath = Path.Combine(AssetPath.DownloadTempRootPath, "Asset");
            AssetPath.IndexDownloadTempPath = Path.Combine(AssetPath.DownloadTempRootPath, "Index");
        }
    }
    public class AssetPath
    {
        public string RootPath { get; set; }
        public string SortedAssetPath { get; set; }
        public string MirrorAssetRootPath { get; set; }
        public string RevMirrorAssetPath { get; set; }
        public string GameMirrorAssetPath { get; set; }
        public string DownloadTempRootPath { get; set; }
        public string ApkDownloadTempPath { get; set; }
        public string AssetDownloadTempPath { get; set; }
        public string IndexDownloadTempPath { get; set; }
    }

    public class IndexPath
    {
        public string RootPath { get; set; }
    }

    public class ConfigurePath
    {
        public string RootPath { get; set; }
    }

}