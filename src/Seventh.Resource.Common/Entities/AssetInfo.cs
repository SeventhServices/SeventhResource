using System;

namespace Seventh.Resource.Common.Entities
{
    public class AssetInfo
    {
        public string Name { get; set; }
        public string SortedName { get; set; }
        public int Revision { get; set; }
        public string Extension { get; set; }
        public string SortedExtension { get; set; }
        public long Size { get; set; }
        public long SortedSize { get; set; }
        public string Path { get; set; }
        public string SortedPath { get; set; }
        public bool Encrypted { get; set; }
        public bool IsExist => Size != 0;
        public bool IsSortedExist => SortedSize != 0;

        public AssetInfo SetRevision(int revision)
        {
            Revision = revision;
            return this;
        }

        public AssetInfo InitialAsset(AssetFileInfo mirrorFileInfo, AssetFileInfo sortedFileInfo, bool encrypted)
        {
            if (mirrorFileInfo is null) throw new ArgumentNullException(nameof(mirrorFileInfo));
            if (sortedFileInfo is null) throw new ArgumentNullException(nameof(sortedFileInfo));

            Name = mirrorFileInfo.Name;
            Size = mirrorFileInfo.Size;
            Path = mirrorFileInfo.Path;
            Extension = mirrorFileInfo.Extension;

            SortedName = sortedFileInfo.Name;
            SortedSize = sortedFileInfo.Size;
            SortedPath = sortedFileInfo.Path;
            SortedExtension = sortedFileInfo.Extension;

            Revision = mirrorFileInfo.Revision;
            Encrypted = encrypted;
            return this;
        }

        public AssetInfo InitialUnsortedAsset(AssetFileInfo fileInfo)
        {
            if (fileInfo is null) throw new ArgumentNullException(nameof(fileInfo));

            Name = fileInfo.Name;
            Size = fileInfo.Size;
            Path = fileInfo.Path;
            Extension = fileInfo.Extension;

            SortedName = Name;
            SortedSize = Size;
            SortedPath = Path;
            SortedExtension = Extension;

            Revision = fileInfo.Revision;
            Encrypted = false;
            return this;
        }
    }
}
