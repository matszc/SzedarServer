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
            if (match.NextLoserMatchCode == null)
            {
                //   _tournamentService
            }
            var nextMatch = _treeRepository.GetMatchByCode(match.MatchCode, match.TournamentId);
            var nextLoserMatch = _treeRepository.GetMatchByCode(match.NextLoserMatchCode, match.TournamentId);
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