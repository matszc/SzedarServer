using System;
using System.Threading.Tasks;
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

        public SingleEliminationController(ISingleEliminationService singleEliminationService, ITournamentRepository tournamentRepository)
        {
            _singleEliminationService = singleEliminationService;
            _tournamentRepository = tournamentRepository;
        }

        [HttpGet("tree/{id}")]
        public  IActionResult GetSingleEliminationTree(string id)
        {
            var guid = Guid.Parse(id);
            
            if (!CheckTournament(guid))
            {
                return NotFound();
            }
            
            var res = _singleEliminationService.GetSingleEliminationTree(guid);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
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
        public async Task<IActionResult> AddResult([FromBody] MatchDTO match)
        {
            await _singleEliminationService.UpdateResult(match);
            return Ok();
        }

        private bool CheckTournament(Guid id)
        {
            var tournament = _tournamentRepository.GetRawTournament(id);

            if (tournament == null || tournament.Type != TournamentsTypes.SingleElimination)
            {
                return false;
            }

            return true;
        }
    }
}