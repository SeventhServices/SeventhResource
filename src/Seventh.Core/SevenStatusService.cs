namespace Seventh.Core
{
    public class SevenStatusService
    {
        private readonly SeventhServiceLocation _location;

        public SevenStatusService(SeventhServiceLocation location)
        {
            _location = location;
        }

        public string GetVersionInfoUrl()
        {
            return $"{_location.StatusServiceUrl}info/version";
        }
    }
}