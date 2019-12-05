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

            res.SwissTable = CreateSwissTable(tournament.Matches);

            res.Rounds = GenerateRounds(tournament);

            return res;
        }

        private IEnumerable<RoundDTO> GenerateRounds(Tournament tournament)
        {
            throw new NotImplementedException();
        }

        private SwissTableDTO[] CreateSwissTable(IEnumerable<Match> matches)
        {
            throw new NotImplementedException();
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