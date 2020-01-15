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
    public class TournamentRepository: ITournamentRepository
    {
        private readonly DataBaseContext _context;

        public TournamentRepository(DataBaseContext context)
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
        public Tournament GetTournament(Guid id)
        {
            return _context.Tournaments.Where(t => t.Id == id)
                .Include(b => b.Players)
                .ThenInclude(x => x.Results)
                .Include(b => b.Matches)
                .ThenInclude(m => m.Result)
                .FirstOrDefault();
        }

        public IEnumerable<Tournament> GetAllUserTournaments(Guid userId)
        {
            return _context.Tournaments.Where(t => t.UserId == userId);
        }

        public Match GetMatch(Guid id)
        {
            return _context.Matches.Where(t => t.Id == id)
                .Include(b => b.Result)
                .ThenInclude(r => r.Player)
                .SingleOrDefault();
        }

        public Tournament GetRawTournament(Guid id)
        {
            return _context.Tournaments.SingleOrDefault(t => t.Id == id);
        }

        public async Task UpdateResult(Result result1, Result result2)
        {
            _context.Results.Update(result1);
            _context.Results.Update(result2);
            
            await _context.SaveChangesAsync();
        }

        public async Task AddResult(Result result)
        {
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteResult(Result result)
        {
            _context.Results.Remove(result);
            await _context.SaveChangesAsync();
        }
    }
}