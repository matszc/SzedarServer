using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.Services
{
    public class SwissService : ISwissService
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
                if (players.Exists(p => p.Nick == player))
                {
                    continue;
                }

                var newPlayer = new Player(player, newTournament);
                players.Add(newPlayer);
            }

            newTournament.Players = players;

            var firstRound = GenerateFirstRound(newTournament);

            await _tournamentRepository.CreateTournamentAsync(newTournament, players, firstRound.Matches,
                firstRound.Results);

            return newTournament;
        }

        public SwissDTO GetTournamentDataAsync(Guid tournamentId)
        {
            var tournament = _tournamentRepository.GetTournament(tournamentId);

            var res = new SwissDTO()
            {
                NumberOfRounds = tournament.NumberOfRounds,
                OwnerId = tournament.UserId,
                SwissTable = CreateSwissTable(tournament),
                Rounds = GetRounds(tournament),
            };

            return res;
        }

        public async Task AddResultAsync(Guid matchId, MatchDTO match)
        {
            var oldMatch = _tournamentRepository.GetMatch(matchId);
            if (!oldMatch.EditAble)
            {
                throw new ValidationException("Match is closed");
            }

            if (match.Player1Score == match.Player2Score)
            {
                throw new ValidationException("Invalid score");
            }

            var res1 = oldMatch.Result.First(r => r.Player.Nick == match.Player1);
            var res2 = oldMatch.Result.First(r => r.Player.Nick == match.Player2);

            res1.Score = match.Player1Score;
            res2.Score = match.Player2Score;
            res1.Win = match.Player2Score < match.Player1Score;
            res2.Win = match.Player2Score > match.Player1Score;

            await _tournamentRepository.UpdateResult(res1, res2);
        }

        public async Task MoveNextRound(Guid tournamentId)
        {
            var tournament = _tournamentRepository.GetTournament(tournamentId);

            if (tournament.NumberOfRounds == tournament.CurrentRound)
            {
                throw new ValidationException("Tournament is finished");
            }

            var swissTable = CreateSwissTable(tournament);
            var nextRound = GenerateNextRound(tournament, swissTable);

            await _swissRepository.AddMatches(nextRound.Matches, nextRound.Results);
            await _swissRepository.MoveNextRound(tournamentId, tournament.CurrentRound);
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
                        EditAble = match.EditAble,
                    };
                    if (matchDto.Player1 == matchDto.Player2)
                    {
                        matchDto.Player2 = "{bay}";
                        matchDto.Player2Score = 0;
                    }

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
                var t1 = 0;
                var points = 0;
                var wins = 0;
                var loses = 0;
                var matchesWon = 0;
                var matchesLost = 0;
                var opponentsList = new List<Guid>();

                foreach (var result in player.Results)
                {
                    points += result.Win ? 2 : 0;
                    wins += result.Score;

                    var match = tournament.Matches.SingleOrDefault(m => m.Id == result.MatchId);

                    var secondResult = match?.Result?.SingleOrDefault(r => r.Id != result.Id);

                    if (result.Win)
                    {
                        matchesWon++;
                    }

                    if (secondResult != null)
                    {
                        opponentsList.Add(secondResult.PlayerId);
                        t1 += GetPlayerWins(secondResult.PlayerId,
                            tournament.Players.Single(p => p.Id == secondResult.PlayerId).Results);
                        if (secondResult.Win && !result.Win)
                        {
                            matchesLost++;
                        }
                        loses += secondResult.Score;
                    }
                }

                res.Add(new SwissTableDTO()
                {
                    Id = player.Id,
                    Player = player.Nick,
                    Points = points,
                    Losses = loses,
                    Wins = wins,
                    MatchesLost = matchesLost,
                    MatchesWon = matchesWon,
                    T1 = t1,
                    T3 = wins - loses,
                    oponentsIds = opponentsList
                });
            }

            foreach (var r in res)
            {
                var t2 = 0;
                foreach (var oponent in r.oponentsIds)
                {
                    var o = res.Single(i => i.Id == oponent);
                    t2 += o.T1;
                }

                r.T2 = t2;
            }

            var list = res.OrderBy(p => p.Points)
                .ThenBy(p => p.T1)
                .ThenBy(p => p.T2)
                .ThenBy(p => p.T3)
                .Reverse()
                .ToList();

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

        private int GetPlayerWins(Guid playerId, IEnumerable<Result> results)
        {
            var res = 0;
            var playerResults = results.Where(r => r.PlayerId == playerId);
            foreach (var result in playerResults)
            {
                if (result.Win)
                {
                    res++;
                }
            }
            return res;
        }

        private SwissRoundModel GenerateNextRound(Tournament tournament, IEnumerable<SwissTableDTO> table)
        {
            var matches = new List<Match>();
            var results = new List<Result>();
                var list = new List<SwissTableDTO>(table);
            while (list.Any())
            {
                var match = new Match(tournament.CurrentRound + 1, tournament.Id);
                var player1 = list[0];
                var i = list.Count() == 1? 0: 1;
                while (CheckIfPlayersPlayedEachOther(tournament.Matches, player1.Id, list[i].Id) && i < list.Count()-1)
                {
                    i++;
                }
                var player2 = list.Count() > i && i != 0? list[i]: null;
                
                var r1 = new Result(player1.Id, match.Id)
                {
                    Win = false,
                    Score = 0,
                };
                var r2 = new Result();
                if (player2 != null)
                {
                    r2.MatchId = match.Id;
                    r2.PlayerId = player2.Id;
                    r2.Win = false;
                    r2.Score = 0;
                    results.Add(r2);
                    list.Remove(player2);
                }
                if (player2 == null)
                {
                    r1.Score = 1;
                    r1.Win = true;
                    match.EditAble = false;
                }
                list.Remove(player1);
                matches.Add(match);
                results.Add(r1);
            }
            
            return new SwissRoundModel(matches, results);
        }

        private bool CheckIfPlayersPlayedEachOther(IEnumerable<Match> matches, Guid player1Id, Guid player2Id)
        {
            //TODO make this function recurrence
            var res = false;
            foreach (var match in matches)
            {
                if (match.Result.First().Id == match.Result.Last().Id && match.Result.Count() > 1)
                {
                    res = true;
                }
            }

            return res;
        }

        private SwissRoundModel GenerateFirstRound(Tournament tournament)
        {
            var matches = new List<Match>();
            var results = new List<Result>();
            var randomGenerator = new Random();
            while (tournament.Players.Any())
            {
                var match = new Match(1, tournament.Id);

                var p1 = tournament.Players.ElementAtOrDefault(randomGenerator.Next(0, tournament.Players.Count() - 1));
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
                    result1.Score = 1;
                    result1.Win = true;
                    match.EditAble = false;
                }

                matches.Add(match);
                results.Add(result1);
            }

            return new SwissRoundModel(matches, results);
        }
    }
}