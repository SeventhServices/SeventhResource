using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Namespace;
using Seventh.Resource.Api.Data.Abstractions;

namespace Seventh.Resource.Api.Data.Repository
{
    class CharacterVoiceRepository : BaseRepository<CharacterVoice>
    {
        public CharacterVoiceRepository(IContextProvider provider) 
            : base(provider)
        {

        }

        public override Task<IEnumerable<CharacterVoice>> GetListAsync()
        {
            return _context;
        }
    }
}
