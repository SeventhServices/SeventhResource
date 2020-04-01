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

        [HttpGet("card/{cardId}")]
        public async Task<ActionResult<Card>> GetCardById(int cardId, 
            [FromServices] IRepository<Card> cardRepository)
        {
            var cards = await cardRepository.GetListAsync();
            var card = cards.FirstOrDefault(c => c.CardId == cardId);
            return Ok(card);
        }

        [HttpGet("characters")]
        public async Task<ActionResult<Character>> GetCharaById( 
            [FromServices] IRepository<Character> characterRepository)
        {
            var characters = await characterRepository.GetListAsync();
            return Ok(characters);
        }

        [HttpGet("character/{charaId}")]
        public async Task<ActionResult<Character>> GetCharaById(int charaId, 
            [FromServices] IRepository<Character> characterRepository)
        {
            var characters = await characterRepository.GetListAsync();
            var character = characters.FirstOrDefault(c => c.CharacterId == charaId);
            return Ok(character);
        }
    }
}
