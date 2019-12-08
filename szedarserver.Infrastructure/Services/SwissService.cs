using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.Services
{
    public class SwissService: ISwissService
    {
        private readonly ISwissRepository _swissRepository;
        private readonly ITournamentRepository _tournamentRepository;

        public SwissService(ISwissRepository swissRepository, ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;
            _swissRepository = swissRepository;
        }
        public async Task<Tournament> CreateSwissTournamentAsync(RegisterTournamentModel tournament, Guid userId)
        {
            var newTournament = new Tournament(tournament.Name, tournament.Rounds, userId, tournament.Type);

            var players = new List<Player>();
            foreach (var player in tournament.Players)
            {
                var newPlayer = new Player(player, newTournament);
                players.Add(newPlayer);
            }

            newTournament.Players = players;

            var firstRound = GenerateFirstRound(newTournament);
            
            await _swissRepository.CreateTournamentAsync(newTournament, players, firstRound.Matches, firstRound.Results);

            return newTournament;

        }

        public SwissDTO GetTournamentDataAsync(Guid tournamentId)
        {
            var tournament = _tournamentRepository.GetTournament(tournamentId);
            
            SwissDTO res = new SwissDTO();

            res.SwissTable = CreateSwissTable(tournament);

           res.Rounds = GetRounds(tournament);

            return res;
        }

        private IEnumerable<RoundDTO> GetRounds(Tournament tournament)
        {
            var res = new List<RoundDTO>();
            for (var i = 1; i <= tournament.CurrentRound; i++)
            {
                var matchesList = new List<MatchDTO>();
                var matches = tournament.Matches.Where(m => m.Round == i).ToList();
                foreach (var match in matches)
                {
                    var matchDto = new MatchDTO
                    {
                        Id = match.Id,
                        Player1 = match.Result.First().Player.Nick,
                        Player1Score = match.Result.First().Score,
                        Player2 = match.Result.Last().Player.Nick,
                        Player2Score = match.Result.Last().Score,
                    };
                    matchesList.Add(matchDto);
                }
                res.Add(new RoundDTO
                {
                    MatchDtos = matchesList,
                });
            }

            return res;
        }

        private IEnumerable<SwissTableDTO> CreateSwissTable(Tournament tournament)
        {
            var res = new List<SwissTableDTO>();
            foreach (var player in tournament.Players)
            {
                var swissTableDto = new SwissTableDTO(player.Nick);

                var points = 0;
                var wins = 0;
                var loses = 0;
                
                foreach (var result in player.Results)
                {
                    points += result.Win ? 2 : 0;
                    wins += result.Score;

                    var match = tournament.Matches.SingleOrDefault(m => m.Id == result.MatchId);

                    var losesScore = match?.Result?.SingleOrDefault(r => r.MatchId != result.MatchId);

                    if(losesScore != null)
                    {
                        loses += losesScore.Score;   
                    }
                }
                swissTableDto.Points = points;
                swissTableDto.MatchesLost = loses;
                swissTableDto.MatchesWon = wins;
                res.Add(swissTableDto);
            }

            var list =  res.OrderBy(p => p.Points).Reverse().ToList();

            var satrtPositon = 1;
            
            for (var i = 0; i < list.Count(); i++)
            {
                var item = list[i];

                item.Position = satrtPositon;
                if (i < list.Count() - 1)
                {
                    if (item.Points > list[i + 1].Points)
                    {
                        satrtPositon++;
                    } 
                }
            }

            return list;
        }

        private SwissRoundModel GenerateNextRound(SwissTableDTO table, IEnumerable<Match> matches)
        {
            throw new NotImplementedException();
        }

        private SwissRoundModel GenerateFirstRound(Tournament tournament)
        {
            var matches = new List<Match>();
            var results = new List<Result>();
            var randomGenerator = new Random();
            while (tournament.Players.Any())
            {
                var match = new Match(1, tournament.Id);
                
                var p1 = tournament.Players.ElementAtOrDefault(randomGenerator.Next(0, tournament.Players.Count() -1));
                tournament.Players = tournament.Players.Where(p => p.Id != p1.Id).ToList();
                
                var result1 = new Result(p1.Id, match.Id);
                
                var p2 = tournament.Players.ElementAtOrDefault(randomGenerator.Next(0, tournament.Players.Count()));

                if (p2 != null)
                {
                    tournament.Players = tournament.Players.Where(p => p.Id != p2.Id).ToList();
                    var result2 = new Result(p2.Id, match.Id);
                    results.Add(result2);
                }
                if (p2 == null)
                {
                    results.Add(new Result
                    {
                        MatchId = match.Id
                    });
                    result1.Score = 1;
                    result1.Win = true;
                }

                matches.Add(match);
                results.Add(result1);
            }
            return new SwissRoundModel(matches, results);
        }
    }
}