using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.IServices;
using System.Security.Cryptography;
using szedarserver.Infrastructure.Extensions;
using szedarserver.Core.Domain;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using szedarserver.Infrastructure.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Internal;

namespace szedarserver.Infrastructure.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IJwtExtension _jwtExtension;
        private readonly ITournamentRepository _tournamentRepository;
        public UserService(IUserRepository userRepository, IMapper mapper, IJwtExtension jwtExtension,
            ITournamentRepository tournamentRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtExtension = jwtExtension;
            _tournamentRepository = tournamentRepository;
        }

        public async Task<AccountDTO> LoginAsync(LoginModel user)
        {
            user.Password = HashExtension.HashPassword(user.Password);
            var User = await _userRepository.GetUserAsync(_mapper.Map<User>(user));
            if(User == null)
            {
                return null;
            }
            var account = _mapper.Map<AccountDTO>(User);

            account.Token = _jwtExtension.CreateToken(account);

            return account;

        }

        public async Task<AccountDTO> LoginFbAsync(FbUserModel user)
        {
            var userFromDb = await _userRepository.GetUserByFbIdAsync(user.FbId);
            if (userFromDb == null)
            {
                userFromDb = new User(user.Email, user.Login, user.FbId, "");
                await _userRepository.AddUserAsync(userFromDb);
            }
            var account = _mapper.Map<AccountDTO>(userFromDb);
            account.Token = _jwtExtension.CreateToken(account);
            return account;
        }

        public IEnumerable<TournamentDTO> GetAllAvailableTournaments(Guid userId, GameTypes gameType)
        {
            var tournaments = _userRepository.GetAllTournaments(userId, gameType);         
            var res = new List<TournamentDTO>();

            foreach (var tournament in tournaments)
            {
                var item = _mapper.Map<TournamentDTO>(tournament);
                item.NumberOfPlayers = tournament.Players.Count();
                res.Add(item);
            }

            return res;
        }

        public async Task JoinTournament(Guid userId, Tournament tournament)
        {
            var user = _userRepository.GetUserById(userId);
            await _userRepository.AddPlayerToTournament(new Player(user.Login, tournament, userId));
        }

        public IEnumerable<RankingDTO> GetPlayersRanking(Guid userId)
        {
            var userTournaments = _tournamentRepository.GetAllUserTournaments(userId);
            var tournamentsIds = userTournaments.Select(p => p.Id);
            var players = _userRepository.GetAllUserPlayers(tournamentsIds);

            return CreateRanking(players);
        }

        public async Task RegisterAsync(UserRegisterModel user)
        {
            var userFromDbByLogin = await _userRepository.GetByEmailAsync(user.Email);
            if(userFromDbByLogin != null)
            {
                throw new ValidationException("Login already exists");
            }
            var userFromDbByEmail = await _userRepository.GetByLoginAsync(user.Login);
            if(userFromDbByEmail != null)
            {
                throw new ValidationException("Email already exists");
            }

            if (user.Password.Length <= 4)
            {
                throw new ValidationException("Password too short");
            }
            var newUser = new User(user.Email, HashExtension.HashPassword(user.Password), user.Login);
            await _userRepository.AddUserAsync(newUser);

        }

        private List<RankingDTO> CreateRanking(List<Player> players)
        {
            var res = new List<RankingDTO>();
            var uniquePlayers = players.GroupBy(p => p.Nick)
                .Select(g => g.First())
                .ToList();

            foreach (var player in uniquePlayers)
            {
                var samePlayers = players.FindAll(p => p.Nick == player.Nick);
                var results = samePlayers.SelectMany(p => p.Results).ToList();
                var resultsId = results.Select(r => r.Id);

                res.Add( new RankingDTO()
                {
                    Id = player.Id,
                    Player = player.Nick,
                    MatchesWon = results.FindAll(r => r.Win).Count,
                    Wins = results.Select(r => r.Score).Sum(),
                    Points = results.FindAll(r => r.Win).Count * 2,
                    Losses = results.FindAll(r => !r.Win && r.Match.Result
                                                      .SingleOrDefault(s => !resultsId.Contains(s.Id)) != null)
                        .FindAll(m => m.Match.Result.Single(r => !resultsId.Contains(r.Id)).Win).Count(),
                    MatchesLost = results.FindAll(r => !r.Win && r.Match.Result
                                                           .SingleOrDefault(s => !resultsId.Contains(s.Id)) != null)
                        .FindAll(m => m.Match.Result.Single(u => !resultsId.Contains(u.Id)) != null)
                        .Select(z => z.Score).Sum(),
                });
            }

            var i = 1;

            RankingDTO prevItem = null;
            
            foreach (var item in res.OrderBy(r => r.Points))
            {
                item.Position = i;
                if (prevItem != null && prevItem.Points < item.Points)
                {
                    i++;
                }

                prevItem = item;
            }

            return res;
        }

    }
}
