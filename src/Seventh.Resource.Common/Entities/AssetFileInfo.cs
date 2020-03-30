namespace Seventh.Resource.Common.Entities
{
    public class AssetFileInfo
    {
        public string Name { get; set; }
        public int Revision { get; set; }
        public string Extension => System.IO.Path.GetExtension(Name);
        public long Size { get; set; }
        public string Path { get; set; }
        public bool IsExist => Size != 0;
    }
}