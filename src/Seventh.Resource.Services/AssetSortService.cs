using System.Threading.Tasks;
using Seventh.Resource.Common.Classes.Options;
using Seventh.Resource.Common.Extensions;

namespace Seventh.Resource.Services
{
    public class AssetSortService
    {
        private readonly ResourceLocation _location;

        public AssetSortService(ResourceLocation location)
        {
            _location = location;
        }

        public Task<string> SortAsync(string fileName)
        {
            return Task.FromResult(_location.PathOption.AssetPath.SortedAssetPath.AppendPath(fileName));
        }

    }
}