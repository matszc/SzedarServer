using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using szedarserver.Core.Domain;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Services;

namespace szedarserver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SwissController : ControllerBase
    {
        private readonly ISwissService _swissService;
        
        private Guid UserId => User.Identity.IsAuthenticated ? Guid.Parse(User.Identity.Name) : Guid.Empty;

        public SwissController(ISwissService swissService)
        {
            _swissService = swissService;
        }
        
        [HttpGet(("{id}"))]
        public IActionResult GetSwissInfo(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var guid = Guid.Parse(id);
            var res =  _swissService.GetTournamentDataAsync(guid);

            return Ok(res);
        }
        
        [HttpPatch("match/{id}")]
        
        public async Task<IActionResult> AddMatchResultAsync([FromBody] MatchDTO match, string id)
        {
            await _swissService.AddResultAsync(Guid.Parse(id), match);
            return Ok();
        }

        [HttpPost("round/{id}")]
        public async Task<IActionResult> CreateNextRound(string id)
        {
            await _swissService.MoveNextRound(Guid.Parse(id));
            return Ok();
        }
    }
}