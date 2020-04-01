using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Namespace;
using Seventh.Resource.Api.Data.Abstractions;
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
        public async Task<ActionResult<IEnumerable<Character>>> GetCharacters( 
            [FromServices] IRepository<Character> characterRepository)
        {
            var characters = await characterRepository.GetListAsync();
            return Ok(characters);
        }

        [HttpGet("character/{characterId}")]
        public async Task<ActionResult<IEnumerable<CharacterVoice>>> GetCharaterById(int characterId, 
            [FromServices] IRepository<Character> characterRepository)
        {
            var characters = await characterRepository.GetListAsync();
            var character = characters.FirstOrDefault(c => c.CharacterId == characterId);
            return Ok(character);
        }

        [HttpGet("character/voices")]
        public async Task<ActionResult<IEnumerable<CharacterVoice>>> GetCharaterVoice([FromQuery] int? characterId, 
            [FromServices] IRepository<CharacterVoice> characterVoiceRepository)
        {
            var voices = await characterVoiceRepository.GetListAsync();

            if (characterId.HasValue)
            { 
                var voiceQuery = voices.AsQueryable();
                voiceQuery = voiceQuery.Where( v => v.CharacterId == characterId.Value);
                return Ok(voiceQuery.ToImmutableArray());
            }
            
            return Ok(voices.Take(50));
        }

        [HttpGet("character/voice/{characterVoiceId}")]
        public async Task<ActionResult<CharacterVoice>> GetCharaterVoiceById(int characterVoiceId, 
            [FromServices] IRepository<CharacterVoice> characterRepository)
        {
            var characters = await characterRepository.GetListAsync();
            var character = characters.FirstOrDefault(c => c.CharacterId == characterVoiceId);
            return Ok(character);
        }
    }
}
