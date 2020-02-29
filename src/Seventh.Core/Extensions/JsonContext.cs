using System.Text;

// ReSharper disable once CheckNamespace
namespace System.Net.Http
{
    public class JsonContext : StringContent
    {
        public JsonContext(string jsonContent) : base(jsonContent,Encoding.UTF8, "application/json")
        {
        }

        public JsonContext(string jsonContent, Encoding encoding) : base(jsonContent, encoding, "application/json")
        {
        }

        public JsonContext(string jsonContent, Encoding encoding, string mediaType) : base(jsonContent, encoding, mediaType)
        {

        }
    }
}