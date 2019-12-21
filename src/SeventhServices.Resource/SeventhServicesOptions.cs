using SeventhServices.Resource.Common.Classes.Options;

namespace SeventhServices.Resource
{
    public class SeventhServicesOptions
    {
        public StatusOption StatusOption { get; set; } = new StatusOption();

        public bool UseHttpFactory { get; set; } = true;

    }
}