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

        private readonly ISingleEliminationService _singleEliminationService;
        private Guid UserId => User.Identity.IsAuthenticated ? Guid.Parse(User.Identity.Name) : Guid.Empty;

        public TournamentController(ITournamentService tournamentService, ISwissService swissService, ISingleEliminationService singleEliminationService)
        {
            _swissService = swissService;
            _tournamentService = tournamentService;
            _singleEliminationService = singleEliminationService;

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
                    await _tournamentService.CreateDoubleEliminationTournament(tournament, UserId);
                    break;
                }
                case TournamentsTypes.SingleElimination:
                {
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