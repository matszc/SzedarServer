using System.Threading.Tasks;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;

namespace szedarserver.Infrastructure.Services
{
    public class TournamentService: ITournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IMapper _mapper;

        public TournamentService(ITournamentRepository tournamentRepository, IMapper mapper)
        {
            _tournamentRepository = tournamentRepository;
            _mapper = mapper;
        }

        public IEnumerable<TournamentDTO> GetAllUserTournaments(Guid userId)
        {
            var tournaments = _tournamentRepository.GetAllUserTournaments(userId);
            var res = new List<TournamentDTO>();

            foreach (var tournament in tournaments)
            {
                res.Add(_mapper.Map<TournamentDTO>(tournament));
            }

            return res.OrderBy(r => r.CreationDate).Reverse();
        }

        public TournamentParts CreateUpperTree(RegisterTournamentModel tournamentData,
            Guid userId)
        {
            var tournament = new Tournament(tournamentData.Name, userId, tournamentData.Type);

            var matches = CreateMatches(tournamentData.Players.Length, tournament.Id);

            var players = new List<Player>();
            var results = new List<Result>();

            foreach (var player in tournamentData.Players)
            {
                if (players.Find(pl => pl.Nick == player) != null)
                {
                    continue;
                }
                var p = new Player(player, tournament);
                results.Add(new Result()
                {
                    PlayerId = p.Id,
                    Score = 0,
                    Win = false,
                });
                players.Add(p);
            }

            var firstRoundIds = matches.Where(m => m.Round == 1).Select(m => m.Id).ToList();

            foreach (var result in results)
            {
                var match = firstRoundIds.FirstOrDefault(m => !results.Where(r => m == r.MatchId).ToList().Any());

                if (match != Guid.Empty)
                {
                    result.MatchId = match;
                }
                else
                {
                    match = firstRoundIds.First(m => results.Where(r => m == r.MatchId).ToList().Count == 1);
                    result.MatchId = match;
                }
            }
            
            var newResults = new List<Result>();

            foreach (var result in results)
            {
                var otherResult = results.Where(r => r.MatchId == result.MatchId).ToList();

                if (otherResult.Count() < 2)
                {
                    result.Score = 1;
                    result.Win = true;
                    var matchCode = matches.Single(m => m.Id == result.MatchId).NextMachCode;
                    var matchId = matches.Single(m => m.MatchCode == matchCode).Id;
                    newResults.Add(new Result(result.PlayerId, matchId));
                }

                matches.First(m => m.Id == result.MatchId).EditAble = false;
            }
            
            return new TournamentParts()
            {
                Tournament = tournament,
                Matches = matches,
                Results = results.Concat(newResults),
                Players = players
            };
        }

        private List<Match> CreateMatches(int numberOfPlayers, Guid tournamentId)
        {
            var res = new List<Match>();

            if (numberOfPlayers % 2 == 1)
            {
                numberOfPlayers++;
            }

            var roundCounter = (int) Math.Ceiling(Math.Log2(numberOfPlayers));

            var matchCount = 1;

            for (int i = roundCounter; i > 0; i--)
            {
                var prevRoundList = res.FindAll(m => m.Round == i + 1).ToList();

                for (int j = 0; j < matchCount; j++)
                {
                    var matchCode = GenerateMatchCode(i, j + 1);
                    var match = new Match(matchCode, i, tournamentId);

                    if (prevRoundList.Count != 0)
                    {
                        var tmp = j;
                        if (j % 2 == 1)
                        {
                            tmp--;
                        }

                        match.NextMachCode = prevRoundList[tmp / 2].MatchCode;
                    }

                    prevRoundList.Add(match);
                    res.Add(match);
                }

                matchCount *= 2;
            }

            return res;
        }
        
        private string GenerateMatchCode(int round, int matchNumber)
        {
            string res = (Convert.ToChar(round + 64)).ToString();
            res += matchNumber.ToString();
            return res;
        }
    }
}