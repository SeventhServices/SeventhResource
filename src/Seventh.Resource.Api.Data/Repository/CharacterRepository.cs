using System.Collections.Generic;
using System.Threading.Tasks;
using Seventh.Resource.Api.Data.Abstractions;
using Seventh.Resource.Database.Entity;

namespace Seventh.Resource.Api.Data.Repository
{
    public class CharacterRepository : BaseRepository<Character>
    {
        public CharacterRepository(IContextProvider provider) 
            : base(provider)
        {

        }
        public override Task<IEnumerable<Character>> GetListAsync()
        {
            return _context;
        }
    }
}
