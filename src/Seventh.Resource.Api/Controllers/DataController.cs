using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seventh.Resource.Database.Entity;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController
    {
        [Route("card/{cardId}")]
        public async Task<Card> GetCardById(int cardId)
        {
            return new Card();
        }
    }
}
