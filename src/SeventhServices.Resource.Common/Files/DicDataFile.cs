using System.Collections.Generic;
using System.IO;
using System.Text;
using SeventhServices.Resource.Common.Abstractions;

namespace SeventhServices.Resource.Common.Files
{
    public class FileDictionary : Dictionary<string, string>, IFileDictionary
    {

        private const int IntShift = 8;

        private const int IntLength = 4;

        public string FilePath { get; set; }

        public FileDictionary(string filePath)
        {
            FilePath = filePath;
            if (File.Exists(filePath)) return;
            try
            {
                var bytes = new Queue<byte>(File.ReadAllBytes(filePath));
                var @int = GetInt(bytes);
                for (var i = 0; i < @int; i++)
                {
                    Add(GetString(bytes), GetString(bytes));
                }
            }
            catch
            {
                Clear();
                throw;
            }
        }

        public void Save()
        {
            var list = new List<byte>();
            SetInt(list, Count);
            using (var enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var (key, value) = enumerator.Current;
                    SetString(list, key);
                    SetString(list, value);
                }
            }
            File.WriteAllBytes(FilePath, list.ToArray());
        }

        public void Delete()
        {
            File.Delete(FilePath);
        }

        private static int GetInt(Queue<byte> bytes)
        {
            var num = 0;
            for (var i = 0; i < IntLength; i++)
            {
                num = (num << IntShift | bytes.Dequeue());
            }
            return num;
        }

        private static void SetInt(ICollection<byte> bytes, int n)
        {
            for (var num = IntLength - 1; num >= 0; num--)
            {
                bytes.Add((byte)(n >> num * IntShift));
            }
        }

        private static string GetString(Queue<byte> bytes)
        {
            var @int = GetInt(bytes);
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < @int; i++)
            {
                stringBuilder.Append((char)bytes.Dequeue());
            }
            return stringBuilder.ToString();
        }

        private static void SetString(ICollection<byte> bytes, string value)
        {
            SetInt(bytes, value.Length);
            foreach (var c in value)
            {
                bytes.Add((byte)c);
            }
        }
    }
}