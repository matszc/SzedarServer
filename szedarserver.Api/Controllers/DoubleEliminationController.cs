using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.IServices;

namespace szedarserver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoubleEliminationController : ControllerBase
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IDoubleEliminationService _doubleEliminationService;
        private readonly IUserRepository _userRepository;
        private Guid UserId => User.Identity.IsAuthenticated ? Guid.Parse(User.Identity.Name) : Guid.Empty;
        public DoubleEliminationController(IUserRepository userRepository, ITournamentRepository tournamentRepository,
            IDoubleEliminationService doubleEliminationService)
        {
            _userRepository = userRepository;
            _tournamentRepository = tournamentRepository;
            _doubleEliminationService = doubleEliminationService;
        }

        [HttpGet("{id}")]

        public IActionResult GetTournament(string id)
        {
            var guid = Guid.Parse(id);
            
            if (!CheckTournament(guid))
            {
                return NotFound();
            }

            var res = _doubleEliminationService.GetMatches(guid);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }

        [HttpPost("match/{id}")]
        [Authorize]
        public async Task<IActionResult> AddResult(Guid id, [FromBody] MatchDTO match)
        {
            var ownerId = _userRepository.GetUserIdByMatchId(id);
            if (ownerId.Result != UserId)
            {
                return Forbid();
            }
            if (match.Player2Score == match.Player1Score)
            {
                return BadRequest("Match can't be draw");
            }

            await _doubleEliminationService.AddResult(id, match);
            return Ok();
        }
        
        private bool CheckTournament(Guid id)
        {
            var tournament = _tournamentRepository.GetRawTournament(id);

            if (tournament == null || tournament.Type != TournamentsTypes.DoubleElimination || tournament.Open)
            {
                return false;
            }

            return true;
        }
    }
}