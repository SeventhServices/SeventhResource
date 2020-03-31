using System.IO;

namespace Seventh.Resource.Common.Options
{
    public class PathOption
    {
        public string RootPath { get; set; }
        public AssetPath AssetPath { get; set; }
        public IndexPath IndexPath { get; set; }

        public PathOption(string rootPath)
        {
            RootPath = rootPath;
            IndexPath = new IndexPath
            {
                RootPath = Path.Combine(RootPath, "index")
            };
            AssetPath = new AssetPath
            {
                RootPath = Path.Combine(RootPath, "asset"),
                TempRootPath = Path.Combine(RootPath, "temp"),
            };
            AssetPath.SortedAssetPath = Path.Combine(AssetPath.RootPath, "sorted");
            AssetPath.MirrorAssetRootPath = Path.Combine(AssetPath.RootPath, "mirror");
            AssetPath.RevMirrorAssetPath = Path.Combine(AssetPath.MirrorAssetRootPath, "revisons");
            AssetPath.GameMirrorAssetPath = Path.Combine(AssetPath.MirrorAssetRootPath, "game");
            AssetPath.BasicMirrorAssetPath = Path.Combine(AssetPath.MirrorAssetRootPath, "basic");
            AssetPath.ApkTempPath = Path.Combine(AssetPath.TempRootPath, "apk");
            AssetPath.AssetTempPath = Path.Combine(AssetPath.TempRootPath, "aseet");
            AssetPath.IndexTempPath = Path.Combine(AssetPath.TempRootPath, "index");
        }
    }
    public class AssetPath
    {
        public string RootPath { get; set; }
        public string SortedAssetPath { get; set; }
        public string MirrorAssetRootPath { get; set; }
        public string RevMirrorAssetPath { get; set; }
        public string GameMirrorAssetPath { get; set; }
        public string BasicMirrorAssetPath { get; set; }
        public string TempRootPath { get; set; }
        public string ApkTempPath { get; set; }
        public string AssetTempPath { get; set; }
        public string IndexTempPath { get; set; }
    }

    public class IndexPath
    {
        public string RootPath { get; set; }
    }

}