using System;

namespace Seventh.Core.Services
{
    public class SevenResourceService
    {
        private readonly SeventhServiceLocation _location;

        public SevenResourceService(SeventhServiceLocation location)
        {
            _location = location;
        }

        public string GetDownloadUrl(string filePath)
        {
            var directories = filePath.Split(new []{"\\","/"},StringSplitOptions.RemoveEmptyEntries);
            return $"{_location.ResourceServiceUrl}{string.Join("/", directories)}";
        }
    }
}