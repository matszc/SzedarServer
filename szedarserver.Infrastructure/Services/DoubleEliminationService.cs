using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.Services
{
    public class DoubleEliminationService : IDoubleEliminationService
    {
        private readonly ITournamentService _tournamentService;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITreeRepository _treeRepository;

        public DoubleEliminationService(ITournamentService tournamentService,
            ITournamentRepository tournamentRepository, ITreeRepository treeRepository)
        {
            _tournamentService = tournamentService;
            _tournamentRepository = tournamentRepository;
            _treeRepository = treeRepository;
        }

        public async Task<Tournament> CreateDoubleElimination(RegisterTournamentModel form, Guid userId)
        {
            var upperBracket = _tournamentService.CreateUpperTree(form, userId);

            var lowerMatches = CreateLowerTree(form.Players.Length, upperBracket.Tournament.Id);

            upperBracket.Matches.OrderBy(m => m.MatchCode).Last().NextMachCode = "Final1";

            var finalMatches = new List<Match>();

            finalMatches.Add(new Match()
            {
                MatchCode = "Final1",
                TournamentId = upperBracket.Tournament.Id,
                Round = upperBracket.Tournament.NumberOfRounds + 1,
                NextMachCode = "Final2"
            });
            finalMatches.Add(new Match()
            {
                Round = upperBracket.Tournament.NumberOfRounds + 2,
                MatchCode = "Final2",
                TournamentId = upperBracket.Tournament.Id
            });

            var lowerMatchesCopy = new List<Match>(lowerMatches);

            foreach (var match in upperBracket.Matches)
            {
                var lowerMatch =
                    lowerMatchesCopy.Find(m => match.Round == 1 ? m.Round == 1 : m.Round == (match.Round - 1) * 2);
                if (match.Round == 1)
                {
                    match.NextLoserMatchCode = lowerMatch.MatchCode;
                    var lowerMatchesWithCode =
                        upperBracket.Matches.Where(m => m.NextLoserMatchCode == lowerMatch.MatchCode);
                    if (lowerMatchesWithCode.Count() > 1)
                    {
                        lowerMatchesCopy.Remove(lowerMatch);
                    }
                }
                else
                {
                    match.NextLoserMatchCode = lowerMatch.MatchCode;
                    lowerMatchesCopy.Remove(lowerMatch);
                }
            }

            await _tournamentRepository.CreateTournamentAsync(upperBracket.Tournament, upperBracket.Players,
                upperBracket.Matches.Concat(lowerMatches).Concat(finalMatches), upperBracket.Results);

            return upperBracket.Tournament;
        }

        public IEnumerable<NodeDTO> GetMatches(Guid tournamentId)
        {
            var tournament = _tournamentRepository.GetTournament(tournamentId);

            if (tournament == null)
            {
                return null;
            }

            return _tournamentService.GetFlatStructure(tournament);
        }

        public async Task AddResult(Guid matchId, MatchDTO result)
        {
            var match = _tournamentRepository.GetMatch(matchId);
            var allMatches = _treeRepository.GetAllMatches(match.TournamentId);
            
            if (match.NextLoserMatchCode == null && (match.MatchCode != "Final1" && match.MatchCode != "Final2"))
            {
                await _tournamentService.UpdateSingleEliminationTree(result);
                return;
            }
            
            var nextMatch = allMatches.SingleOrDefault(m => m.MatchCode == match.NextMachCode);
            var nextLoserMatch = allMatches.SingleOrDefault(m => m.MatchCode == match.NextLoserMatchCode);
            
            if (match.Result.Count() != 2
                || (nextMatch != null && nextMatch.Result != null && nextMatch.Result.SingleOrDefault(r => r.Win) != null)
                || (nextLoserMatch != null && nextLoserMatch.Result != null && nextLoserMatch.Result.SingleOrDefault(r => r.Win) != null))
            {
                throw new ValidationException("Can't update this match");
            }
            
            var p1 = match.Result.Single(r => r.Player.Nick == result.Player1);
            var p2 = match.Result.Single(r => r.Player.Nick == result.Player2);
            
            p1.Score = result.Player1Score;
            p1.Win = result.Player1Score > result.Player2Score;
            
            p2.Score = result.Player2Score;
            p2.Win = result.Player1Score < result.Player2Score;
            
            var winner = result.Player1Score > result.Player2Score ? p1 : p2;
            var loser = result.Player1Score > result.Player2Score ? p2 : p1;
            
            if (nextMatch != null && nextMatch.Result != null && nextMatch.Result.SingleOrDefault(r => r.PlayerId == p1.PlayerId || r.PlayerId == p2.PlayerId) != null)
            {
                await _tournamentRepository.DeleteResult(nextMatch.Result.Single(r => r.PlayerId == p1.PlayerId || r.PlayerId == p2.PlayerId));
            }
            
            if (nextLoserMatch != null && nextLoserMatch.Result != null && nextLoserMatch.Result.SingleOrDefault(r => r.PlayerId == p1.PlayerId || r.PlayerId == p2.PlayerId) != null)
            {
                await _tournamentRepository.DeleteResult(nextLoserMatch.Result.Single(r => r.PlayerId == p1.PlayerId || r.PlayerId == p2.PlayerId));
            }

            if (match.MatchCode == "Final1")
            {
                if (winner.Player.Nick == result.FromLowerBracket)
                {
                    var final2 = allMatches.Single(m => m.MatchCode == "Final2");
                    await _tournamentRepository.UpdateResult(p1, p2);
                    await _tournamentRepository.AddResultsAsync(new List<Result>()
                        {new Result(p1.PlayerId, final2.Id), new Result(p2.PlayerId, final2.Id)});
                }
                else
                {
                    await _tournamentRepository.UpdateResult(p1, p2);
                }
                return;
            }
            
            await _tournamentRepository.UpdateResult(p1, p2);
            
            if (nextMatch == null || nextLoserMatch == null)
            {
                return;
            }

            var results = new List<Result>()
            {
                new Result(winner.PlayerId, nextMatch.Id),
                new Result(loser.PlayerId, nextLoserMatch.Id)
            };

            if (match.Round <= 2)
            {

                await _tournamentRepository.AddResultsAsync(CheckLoserMatches(nextLoserMatch, allMatches, loser.PlayerId, results ));
            }
            else
            {
                await _tournamentRepository.AddResultsAsync(results);
            }
        }

        public async Task StartTournament(Tournament tournament)
        {
            var upperBracket = _tournamentService.StartUpperTree(tournament);

            var lowerMatches = CreateLowerTree(tournament.Players.Count(), tournament.Id);

            upperBracket.Matches.OrderBy(m => m.MatchCode).Last().NextMachCode = "Final1";

            var finalMatches = new List<Match>();

            finalMatches.Add(new Match()
            {
                MatchCode = "Final1",
                TournamentId = tournament.Id,
                Round = tournament.NumberOfRounds + 1,
                NextMachCode = "Final2"
            });
            finalMatches.Add(new Match()
            {
                Round = tournament.NumberOfRounds + 2,
                MatchCode = "Final2",
                TournamentId = tournament.Id
            });

            var lowerMatchesCopy = new List<Match>(lowerMatches);

            foreach (var match in upperBracket.Matches)
            {
                var lowerMatch =
                    lowerMatchesCopy.Find(m => match.Round == 1 ? m.Round == 1 : m.Round == (match.Round - 1) * 2);
                if (match.Round == 1)
                {
                    match.NextLoserMatchCode = lowerMatch.MatchCode;
                    var lowerMatchesWithCode =
                        upperBracket.Matches.Where(m => m.NextLoserMatchCode == lowerMatch.MatchCode);
                    if (lowerMatchesWithCode.Count() > 1)
                    {
                        lowerMatchesCopy.Remove(lowerMatch);
                    }
                }
                else
                {
                    match.NextLoserMatchCode = lowerMatch.MatchCode;
                    lowerMatchesCopy.Remove(lowerMatch);
                }
            }

            await _tournamentRepository.StartTournament(tournament, upperBracket.Matches.Concat(lowerMatches),
                upperBracket.Results);
        }

        private List<Result> CheckLoserMatches(Match baseMatch, List<Match> matches,Guid playerId, List<Result> results)
        {
            if (baseMatch.Round == 2)
            {
                var prevLoserMatch = matches.Single(m => m.NextMachCode == baseMatch.MatchCode);
                var prevUpperMatches = matches.FindAll(m => m.NextLoserMatchCode == prevLoserMatch.MatchCode);
                if (!prevUpperMatches.FindAll(m => m.Result.Count() == 2).Any())
                {
                    var currentResult = results.Single(r => r.MatchId == baseMatch.Id);
                    currentResult.Score = 1;
                    currentResult.Win = true;
                    var nextMatch = matches.Single(m => m.MatchCode == baseMatch.NextMachCode);
                    if (nextMatch.Result.SingleOrDefault(m => m.PlayerId == playerId) == null)
                    {
                        results.Add(new Result(playerId, nextMatch.Id));   
                    }
                }

                return results;
            }
            var sameCodeMatches = matches.Where(m => m.NextLoserMatchCode == baseMatch.MatchCode);
            var match = sameCodeMatches.SingleOrDefault(m => m.Result.Count() < 2);
            if (match != null)
            {
                if (match.Result.First().Win)
                {
                    var currentResult =results.Single(r => r.MatchId == baseMatch.Id);
                    currentResult.Score = 1;
                    currentResult.Win = true;
                    var nextMatch = matches.Single(m => m.MatchCode == baseMatch.NextMachCode);
                    if (nextMatch.Result.SingleOrDefault(m => m.PlayerId == playerId) == null)
                    {
                        results.Add(new Result(playerId, nextMatch.Id));   
                    }
                    return CheckLoserMatches(matches.Single(m => m.MatchCode == baseMatch.NextMachCode), matches, playerId, results);   
                }

                return results;
            }

            return results;
        }

        private List<Match> CreateLowerTree(int numberOfPlayers, Guid tournamentId)
        {
            var res = new List<Match>();

            var matchCounter = (int) Math.Ceiling(Math.Log2(numberOfPlayers));

            var roundCounter = (int) Math.Pow(2, matchCounter) / 4;

            for (int i = 1; roundCounter >= 1; i++)
            {
                for (int j = 1; j <= roundCounter; j++)
                {
                    var match = new Match(GenerateLoserMatchCode(i, j), i, tournamentId);
                    if (roundCounter == 1 && i % 2 == 0)
                    {
                        match.NextMachCode = "Final1";
                        res.Add(match);
                        continue;
                    }

                    if (i % 2 != 1)
                    {
                        match.NextMachCode = j % 2 == 1
                            ? GenerateLoserMatchCode(i + 1, (j + 1) / 2)
                            : match.NextMachCode = GenerateLoserMatchCode(i + 1, j / 2);
                    }
                    else
                    {
                        match.NextMachCode = GenerateLoserMatchCode(i + 1, j);
                    }

                    res.Add(match);
                }

                if (i % 2 == 0)
                {
                    roundCounter /= 2;
                }
            }

            return res;
        }

        private string GenerateLoserMatchCode(int round, int matchNumber)
        {
            string res = (Convert.ToChar(round + 64)).ToString();
            res += matchNumber.ToString();
            return res + "L";
        }
    }
}