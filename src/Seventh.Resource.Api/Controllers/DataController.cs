using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seventh.Resource.Api.Data.Abstractions;
using Seventh.Resource.Api.Data.Repository;
using Seventh.Resource.Database.Entity;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IRepository<Card> _cardRepository;

        public DataController(IRepository<Card> cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [HttpGet("card/{cardId}")]
        public async Task<ActionResult<Card>> GetCardById(int cardId)
        {
            var cards = await _cardRepository.GetListAsync();

            var card = cards.FirstOrDefault( c => c.CardId == cardId);

            return Ok(card);
        }
    }
}
