using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IUserRepository _userRepository;
        private Guid UserId => User.Identity.IsAuthenticated ? Guid.Parse(User.Identity.Name) : Guid.Empty;
        public UserController(IUserService userService, ITournamentRepository tournamentRepository, IUserRepository userRepository)
        {
            _userService = userService;
            _userRepository = userRepository;
            _tournamentRepository = tournamentRepository;
        }
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterModel user)
        {
            await _userService.RegisterAsync(user);
            return Ok();
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginData)
        {
            var res = await _userService.LoginAsync(loginData);
            if(res == null)
            {
                return NotFound("User does not exists");
            }
            return Ok(res);
        }

        [HttpPost("loginFb")]
        public async Task<ActionResult> LoginFbUser([FromBody] FbUserModel fbUser)
        {
            var res = await _userService.LoginFbAsync(fbUser);
            return Ok(res);
        }

        [HttpGet("tournaments")]
        public IActionResult GetAllTournaments([FromQuery(Name = "gameType")] GameTypes gameType )
        {
           var res =  _userService.GetAllAvailableTournaments(UserId, gameType);
            return Ok(res);
        }

        [HttpPost("join/{tournamentId}")]
        [Authorize]
        public async Task<IActionResult> JoinTournament(Guid tournamentId)
        {
            var tournament = _tournamentRepository.GetTournamentWithPlayers(tournamentId);

            var userInTournament = tournament.Players.SingleOrDefault(p => p.UserId == UserId);
            
            if (!tournament.Open || tournament.UserId == UserId ||
                tournament.MaxNumberOfPlayers == tournament.Players.Count() || userInTournament != null)
            {
                return Forbid("You can't join this tournament");
            }

            await _userService.JoinTournament(UserId, tournament);

            return Ok();
        }

        [HttpGet("ranking/{id}")]
        public IActionResult GetPlayersRanking(Guid id)
        {
            return Ok(_userService.GetPlayersRanking(id));
        }

        [HttpGet("profile")]
        public IActionResult GetProfile([FromQuery(Name = "nick")] string nick, [FromQuery(Name = "id")] Guid id)
        {
            var user = id == Guid.Empty ? _userRepository.GetByLogin(nick) : _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            
            return Ok(_userService.GetUserProfile(user));
        }

        [HttpDelete("join/{id}")]
        [Authorize]
        public async Task<IActionResult> LeaveTournament(Guid id)
        {
            var tournament = _tournamentRepository.GetTournamentWithPlayers(id);
            if (tournament.Players.SingleOrDefault(p => p.UserId == UserId) == null)
            {
                return BadRequest("Users not in tournament");
            }

            await _userRepository.DeletePlayer(tournament.Id, UserId);
            
            return Ok();
        }
    }
}