using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seventh.Core.Abstractions.Extend
{
    public interface IJsonHttpExtend
    {
        public Task<TResponseDto> TryJsonGetAsync<TResponseDto>(string url) where TResponseDto : class;
        public Task<TResponseDto> TryJsonGetAsync<TResponseDto>(string url, IEnumerable<KeyValuePair<string, string>> queries) where TResponseDto : class;
        public Task<TResponseDto> TryJsonPostAsync<TResponseDto>(string url) where TResponseDto : class;
        public Task<TResponseDto> TryJsonPostAsync<TRequestDto, TResponseDto>(string url, TRequestDto body) where TRequestDto : class where TResponseDto : class;
    }
}