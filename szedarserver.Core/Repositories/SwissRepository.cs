using System.Collections.Generic;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Core.Domain.Context;
using szedarserver.Core.IRepositories;

namespace szedarserver.Core.Repositories
{
    public class SwissRepository: ISwissRepository
    {
        private readonly DataBaseContext _context;
        public SwissRepository (DataBaseContext context)
        {
            _context = context;
        }
        
        public async Task<Tournament> CreateTournamentAsync (Tournament tournament, IEnumerable<Player> players, IEnumerable<Match> matches, IEnumerable<Result> results)
        {
            _context.Tournaments.Add(tournament);
            foreach (var player in players)
            {
                _context.Players.Add(player);
            }
            foreach (var match in matches)
            {
                _context.Matches.Add(match);
            }
            foreach (var result in results)
            {
                _context.Results.Add(result);
            }
            await _context.SaveChangesAsync();
            return tournament;
        }

/*        public async Task AddPlayersAsync(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                _context.Players.Add(player);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddMatches(IEnumerable<Match> matches)
        {
            foreach (var match in matches)
            {
                _context.Matches.Add(match);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddResults(IEnumerable<Result> results)
        {
            foreach (var result in results)
            {
                _context.Results.Add(result);
            }

            await _context.SaveChangesAsync();
        }*/
    }
}