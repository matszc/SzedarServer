using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Tournament> CreateTournamentAsync(Tournament tournament, IEnumerable<Player> players,
            IEnumerable<Match> matches, IEnumerable<Result> results)
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

        public async Task AddMatches(IEnumerable<Match> matches, IEnumerable<Result> results)
        {
            foreach (var match in matches)
            {
                _context.Matches.Add(match);
            }

            foreach (var result in results)
            {
                _context.Results.Add(result);
            }

            await _context.SaveChangesAsync();
        }

        public async Task MoveNextRound(Guid tournamentId, int currentRound)
        {
            await _context.Matches.Where(m => m.Id == tournamentId && m.Round == currentRound).ForEachAsync(m => m.EditAble = false);

            var tournament = await _context.Tournaments.SingleOrDefaultAsync(t => t.Id == tournamentId);
            
            tournament.CurrentRound = currentRound + 1;

            await _context.SaveChangesAsync();
        }
    }
}