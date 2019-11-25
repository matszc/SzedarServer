using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
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
        private Guid UserId => User.Identity.IsAuthenticated ? Guid.Parse(User.Identity.Name) : Guid.Empty;

        public TournamentController(ITournamentService tournamentService, ISwissService swissService)
        {
            _swissService = swissService;
            _tournamentService = tournamentService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateTournament([FromBody] RegisterTournamentModel tournament)
        {
            switch (tournament.Type)
            {
                case TournamentsTypes.DoubleElimination:
                {
                    await this._tournamentService.CreateDoubleEliminationTournament(tournament, UserId);
                    break;
                }
                case TournamentsTypes.SingleElimination:
                {
                    await _tournamentService.CreateSingleEliminationTournament(tournament, UserId);
                    break;
                }
                case TournamentsTypes.Siwss:
                {
                    await _swissService.CreateSwissTournament(tournament, UserId);
                    break;
                }
            }

            return Ok();
        }
    }
}