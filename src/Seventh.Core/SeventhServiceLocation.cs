using System.Net.Http;

namespace Seventh.Core
{
    public class SeventhServiceLocation
    {
        public SeventhServiceLocation()
        {
            ResourceServiceUrl = "https://resource.t7s.sagilio.net/";
            StatusServiceUrl = "https://status.t7s.sagilio.net/";
            ClientServiceUrl = "https://client.t7s.sagilio.net/";
        }

        public string ResourceServiceUrl { get; private set; }

        public string ClientServiceUrl { get; private set; }

        public string StatusServiceUrl { get; private set; }
    }
}