using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Seventh.Resource.Api.Data.Abstractions;
using Seventh.Resource.Database.Entity;

namespace Seventh.Resource.Api.Data.Repository
{
    public class CardRepository : BaseRepository<Card>
    {
        public CardRepository(IContextProvider provider) 
            : base(provider)
        {

        }
        public override Task<IEnumerable<Card>> GetListAsync()
        {
            return _context;
        }
    }
}
