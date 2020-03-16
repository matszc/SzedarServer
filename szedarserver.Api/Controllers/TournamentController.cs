using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;
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
        public async Task<IActionResult> CreateTournamentAsync([FromBody] RegisterTournamentModel tournament)
        {
            Tournament res = null;
            switch (tournament.Type)
            {
                case TournamentsTypes.DoubleElimination:
                {
                    res = await _doubleEliminationService.CreateDoubleEliminationAsync(tournament, UserId);
                    break;
                }
                case TournamentsTypes.SingleElimination:
                {
                    res = await _singleEliminationService.CreateSingleEliminationTournamentAsync(tournament, UserId);
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

        [HttpPost("create/open")]
        [Authorize]
        public async Task<IActionResult> CreateOpenTournamentAsync([FromBody] RegisterTournamentModel tournament)
        {
            await _tournamentService.CreateOpenTournamentAsync(tournament, UserId);
            return Ok();
        }

        [HttpPost("start/{id}")]
        [Authorize]

        public async Task<IActionResult> StartTournamentAsync(Guid id)
        {
            var tournament = _tournamentRepository.GetTournamentWithPlayers(id);
            if (tournament.UserId != UserId)
            {
                return Forbid("Your not owner of this tournament");
            }

            switch (tournament.Type)
            {
                case TournamentsTypes.Siwss:
                {
                    await _swissService.StartTournamentAsync(tournament);
                    break;
                }
                case TournamentsTypes.DoubleElimination:
                {
                    await _doubleEliminationService.StartTournamentAsync(tournament);
                    break;
                }
                case TournamentsTypes.SingleElimination:
                {
                    await _singleEliminationService.StartTournamentAsync(tournament);
                    break;
                }
                default:
                    return BadRequest();
            }
            
            
            return Ok();
        }

        [HttpGet("GetAll")]
        [Authorize]
        public IActionResult GetAllUserTournaments()
        {
            var res = _tournamentService.GetAllUserTournaments(UserId);

            return Ok(res);
        }

        [HttpPost("addPlayers/{id}")]
        [Authorize]
        public async Task<IActionResult> AddPlayersAsync(Guid id, [FromBody] string[] players)
        {
            var tournament = _tournamentRepository.GetTournamentWithPlayers(id);
            if (UserId != tournament.UserId)
            {
                return Forbid();
            }

            if ((tournament.Players.Count() + players.Length) > tournament.MaxNumberOfPlayers)
            {
                return BadRequest("Max number of players excited");
            }

            await _tournamentService.AddPlayersAsync(players, tournament.Id);
            
            return Ok();
        }

        [HttpDelete("deletePlayer/{id}/{playerId}")]
        [Authorize]
        public async Task<IActionResult> DeletePlayersAsync([FromRoute]Guid id, [FromRoute]Guid playerId)
        {
            var tournament = _tournamentRepository.GetRawTournament(id);
            if (UserId != tournament.UserId)
            {
                return Forbid();
            }

            if (!tournament.Open)
            {
                return BadRequest();
            }

            await _tournamentRepository.RemovePlayerAsync(playerId, id);

            return Ok();
        }

        [HttpPatch("editPlayer/{id}/{playerId}/{nick}")]
        [Authorize]
        public async Task<IActionResult> EditPlayerAsync([FromRoute] Guid id, [FromRoute] Guid playerId, [FromRoute] string nick)
        {
            var t = _tournamentRepository.GetTournamentWithPlayers(id);
            
            if (UserId != t.UserId)
            {
                return Forbid();
            }

            var player = t.Players.Single(i => i.Id == playerId);
            if (player.UserId != Guid.Empty || nick.Length < 2)
            {
                return BadRequest();
            }

            await _tournamentRepository.EditPlayerAsync(playerId, nick);
            
            return Ok();
        }

        [HttpGet("getOpenTournament/{id}")]
        [Authorize]

        public IActionResult GetOpenTournament(Guid id)
        {
            var tournament = _tournamentRepository.GetRawTournament(id);
            if (UserId != tournament.UserId)
            {
                return Forbid();
            }
            if (!tournament.Open)
            {
                return BadRequest();
            }


            return Ok(_tournamentService.GetOpenTorTournament(id));
        }

        [HttpPatch("editOpenTournament/{id}")]
        [Authorize]
        public async Task<IActionResult> EditOpenTournamentAsync([FromRoute] Guid id, [FromBody] OpenTournamentDTO tournament)
        {
            var t = _tournamentRepository.GetRawTournament(id);
            if (UserId != t.UserId)
            {
                return Forbid();
            }
            if (!t.Open)
            {
                return BadRequest();
            }

            await _tournamentService.UpdateOpenTournamentAsync(id, tournament);

            return Ok();
        }

        [HttpDelete("deleteOpenTournament/{id}")]
        [Authorize]
        public async Task<IActionResult> CloseOpenTournament(Guid id)
        {
            var t = _tournamentRepository.GetRawTournament(id);
            if (UserId != t.UserId)
            {
                return Forbid();
            }
            if (!t.Open)
            {
                return BadRequest();
            }

            await _tournamentRepository.CloseOpenTournamentAsync(id);

            return Ok();
        }
        
    }
}