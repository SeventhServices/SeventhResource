namespace Seventh.Resource.Common.Entities
{
    public class AssetFileInfo
    {
        public string FileName { get; set; }
        public string RealFileName { get; set; }
        public int Revision { get; set; }
        public long FileSize { get; set; }
        public long RealFileSize { get; set; }
        public string MirrorSavePath { get; set; }
        public string SortedSavePath { get; set; }
    }
}