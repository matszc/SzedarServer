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

        public DoubleEliminationService(ITournamentService tournamentService, ITournamentRepository tournamentRepository)
        {
            _tournamentService = tournamentService;
            _tournamentRepository = tournamentRepository;
        }

        public async Task<Tournament> CreateDoubleElimination(RegisterTournamentModel form, Guid userId)
        {
            var upperBracket = _tournamentService.CreateUpperTree(form, userId);

            var lowerMatches = CreateLowerTree(form.Players.Length, upperBracket.Tournament.Id);

            upperBracket.Matches.Last().NextMachCode = "Final1";
            
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
                var lowerMatch = lowerMatchesCopy.Find(m => m.Round == match.Round * 2);
                match.NextLoserMatchCode = lowerMatch.MatchCode;
                lowerMatchesCopy.Remove(lowerMatch);
            }

            await _tournamentRepository.CreateTournamentAsync(upperBracket.Tournament, upperBracket.Players,
                upperBracket.Matches.Concat(lowerMatches).Concat(finalMatches), upperBracket.Results);

            return upperBracket.Tournament;
        }

        public IEnumerable<NodeDTO> GetMatches(Guid tournamentId)
        {
            throw new NotImplementedException();
        }

        private List<Match> CreateLowerTree(int numberOfPlayers, Guid tournamentId)
        {
            var res = new List<Match>();

            var matchCounter = (int) Math.Ceiling(Math.Log2(numberOfPlayers));

            for (int i = 1; matchCounter >= 1; i++)
            {
                for (int j = 1; j <= matchCounter; j++)
                {
                    var match = new Match(GenerateLoserMatchCode(i+1, j), i, tournamentId);
                    if (matchCounter == 1)
                    {
                        match.NextMachCode = "Final1";
                        res.Add(match);
                        continue;
                    }
                    if (i % 2 == 1)
                    {
                        match.NextMachCode = j % 2 == 1 ? GenerateLoserMatchCode(i + 2, (j + 1) / 2)
                            : match.NextMachCode = GenerateLoserMatchCode(i + 2, j / 2);
                        
                    }
                    else
                    {
                        match.NextMachCode = GenerateLoserMatchCode(i + 2, j);
                    }
                    res.Add(match);
                }

                if (i % 2 == 1)
                {
                    matchCounter /= 2;
                }
            }

            return res;
        }

        private string GenerateLoserMatchCode(int round, int matchNumber)
        {
            string res = (Convert.ToChar(round + 64)).ToString();
            res += matchNumber.ToString();
            return  res + "L";
        }
    }
}