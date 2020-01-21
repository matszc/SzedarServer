using System;
using Microsoft.AspNetCore.Mvc;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.IServices;

namespace szedarserver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoubleEliminationController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IDoubleEliminationService _doubleEliminationService;
        public DoubleEliminationController(ITournamentService tournamentService, ITournamentRepository tournamentRepository,
            IDoubleEliminationService doubleEliminationService)
        {
            _tournamentService = tournamentService;
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
        
        private bool CheckTournament(Guid id)
        {
            var tournament = _tournamentRepository.GetRawTournament(id);

            if (tournament == null || tournament.Type != TournamentsTypes.DoubleElimination)
            {
                return false;
            }

            return true;
        }
    }
}