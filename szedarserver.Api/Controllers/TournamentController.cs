using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;

        private readonly ISwissService _swissService;

        private readonly ITournamentRepository _tournamentRepository;

        private readonly IDoubleEliminationService _doubleEliminationService;

        private readonly ISingleEliminationService _singleEliminationService;
        private Guid UserId => User.Identity.IsAuthenticated ? Guid.Parse(User.Identity.Name) : Guid.Empty;

        public TournamentController(ITournamentService tournamentService, 
            ISwissService swissService, 
            ISingleEliminationService singleEliminationService, ITournamentRepository tournamentRepository,
            IDoubleEliminationService doubleEliminationService)
        {
            _swissService = swissService;
            _tournamentService = tournamentService;
            _singleEliminationService = singleEliminationService;
            _tournamentRepository = tournamentRepository;
            _doubleEliminationService = doubleEliminationService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateTournament([FromBody] RegisterTournamentModel tournament)
        {
            Tournament res = null;
            switch (tournament.Type)
            {
                case TournamentsTypes.DoubleElimination:
                {
                    res = await _doubleEliminationService.CreateDoubleElimination(tournament, UserId);
                    break;
                }
                case TournamentsTypes.SingleElimination:
                {
                    res = await _singleEliminationService.CreateSingleEliminationTournament(tournament, UserId);
                    break;
                }
                case TournamentsTypes.Siwss:
                {
                    res = await _swissService.CreateSwissTournamentAsync(tournament, UserId);
                    break;
                }
            }

            if (res == null)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet("GetAll")]
        [Authorize]
        public IActionResult GetAllUserTournaments()
        {
            if (UserId == Guid.Empty)
            {
                return Unauthorized();
            }

            var res = _tournamentService.GetAllUserTournaments(UserId);

            return Ok(res);
        }
    }
}