using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public DoubleEliminationService(ITournamentService tournamentService,
            ITournamentRepository tournamentRepository)
        {
            _tournamentService = tournamentService;
            _tournamentRepository = tournamentRepository;
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

            return GetFlatStructure(tournament);
        }

        private IEnumerable<NodeDTO> GetFlatStructure(Tournament tournament)
        {
            var res = new List<NodeDTO>();
            foreach (var match in tournament.Matches)
            {
                var node = new NodeDTO()
                {
                    Id = match.Id,
                    MatchCode = match.MatchCode,
                    Round = match.Round,
                    EditAble = match.Result.Count() == 2,
                    NextMatch = match.NextMachCode != null
                        ? CreateNode(tournament, match.NextMachCode, false, true)
                        : null,
                };
                if (match.Result.Any())
                {
                    node.Player1 = match.Result.First().Player.Nick;
                    node.Player1Score = match.Result.First().Score;
                }

                if (match.Result.Count() > 1)
                {
                    node.Player2Score = match.Result.Last().Score;
                    node.Player2 = match.Result.Last().Player.Nick;
                }

                res.Add(node);
            }

            return res;
        }

        private NodeDTO CreateNode(Tournament tournament, string matchCode, bool flat, bool recursionFlag)
        {
            if (!recursionFlag)
            {
                return null;
            }

            var match = tournament.Matches.First(m => m.MatchCode == matchCode);
            if (match.NextMachCode != null)
            {
                var p1 = match.Result.FirstOrDefault()?.Player.Nick;
                var p1Score = match.Result.FirstOrDefault()?.Score;
                var p2 = match.Result.LastOrDefault()?.Player.Nick;
                var p2Score = match.Result.LastOrDefault()?.Score;

                return new NodeDTO()
                {
                    MatchCode = match.MatchCode,
                    Id = match.Id,
                    Round = match.Round,
                    Player1 = p1,
                    Player1Score = p1Score != null ? p1Score.Value : 0,
                    Player2 = p1 != p2 ? p2 : null,
                    Player2Score = p1 != p2 ? p2Score != null ? p2Score.Value : 0 : 0,
                    EditAble = match.EditAble,
                    NextMatch = CreateNode(tournament, match.NextMachCode, flat, flat)
                };
            }

            var lastNode = new NodeDTO()
            {
                MatchCode = match.MatchCode,
                Id = match.Id,
                Round = match.Round,
                EditAble = match.EditAble,
            };

            if (match.Result.Any())
            {
                lastNode.Player1 = match.Result.First().Player.Nick;
                lastNode.Player1Score = match.Result.First().Score;
            }

            if (match.Result.Count() > 1)
            {
                lastNode.Player2Score = match.Result.Last().Score;
                lastNode.Player2 = match.Result.Last().Player.Nick;
            }

            return lastNode;
        }


        private List<Match> CreateLowerTree(int numberOfPlayers, Guid tournamentId)
        {
            var res = new List<Match>();

            var matchCounter = (int) Math.Ceiling(Math.Log2(numberOfPlayers));

            var roundCounter = (int) Math.Pow(matchCounter, 2) / 4;

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