using System;
using System.Collections.Generic;
using System.Linq;
using Seventh.Resource.Api.Core.Dto.Request;
using Seventh.Resource.Common.Entities;

namespace Seventh.Resource.Api
{
    public static class QueryExtension
    {
        public static IEnumerable<AssetFileInfo> Query(
            this IEnumerable<AssetFileInfo> fileInfos, QueryFileParamsDto queryDto)
        {
            if (queryDto.Extension == null && queryDto.Revision == null)
            {
                return fileInfos;
            }

            var infoQuery = fileInfos.AsQueryable();
            if (queryDto.Revision != null)
            {
                var revList = queryDto.Revision.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var revs = new List<int>();
                foreach (var revstring in revList)
                {
                    if (int.TryParse(revstring, out var rev))
                    {
                        revs.Add(rev);
                    }
                }
                infoQuery = infoQuery.Where(info => revs.Any(r => r == info.Revision));
            }
            if (queryDto.Extension != null)
            {
                infoQuery = infoQuery.Where(info => info.Extension.Equals(queryDto.Extension));
            }

            return infoQuery;
        }

        public static IEnumerable<AssetInfo> Query(
            this IEnumerable<AssetInfo> assetInfos, QueryAssetParamsDto queryDto)
        {
            if (queryDto.Extension == null)
            {
                return assetInfos;
            }

            var infoQuery = assetInfos.AsQueryable();

            if (queryDto.Extension != null)
            {
                infoQuery = infoQuery.Where(info => info.SortedFileInfo.Extension.Equals(queryDto.Extension));
            }

            return infoQuery;
        }
    }
}
