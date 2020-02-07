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
    public class SingleEliminationController : ControllerBase
    {
        private readonly ISingleEliminationService _singleEliminationService;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITournamentService _tournamentService;
        private readonly IUserRepository _userRepository;
        private Guid UserId => User.Identity.IsAuthenticated ? Guid.Parse(User.Identity.Name) : Guid.Empty;

        public SingleEliminationController(ISingleEliminationService singleEliminationService, ITournamentRepository tournamentRepository,
            ITournamentService tournamentService, IUserRepository userRepository)
        {
            _singleEliminationService = singleEliminationService;
            _tournamentRepository = tournamentRepository;
            _tournamentService = tournamentService;
            _userRepository = userRepository;
        }

        [HttpGet("flat/{id}")]
        public IActionResult GetFlatStructure(string id)
        {
            var guid = Guid.Parse(id);
            
            if (!CheckTournament(guid))
            {
                return NotFound();
            }

            var res = _singleEliminationService.GetFlatStructure(guid);

            if (res == null)
            {
                return NotFound();
            }
            
            return Ok(res);
        }

        [HttpPatch("match")]
        [Authorize]
        public async Task<IActionResult> AddResult([FromBody] MatchDTO match)
        {
            var ownerId = _userRepository.GetUserIdByMatchId(match.Id);
            if (ownerId.Result != UserId)
            {
                return Forbid();
            }
            if (match.Player2Score == match.Player1Score)
            {
                return BadRequest("Match can't be draw");
            }
            await _tournamentService.UpdateSingleEliminationTree(match);
            return Ok();
        }

        private bool CheckTournament(Guid id)
        {
            var tournament = _tournamentRepository.GetRawTournament(id);

            if (tournament == null || (tournament.Type != TournamentsTypes.SingleElimination || tournament.Open))
            {
                return false;
            }

            return true;
        }
    }
}