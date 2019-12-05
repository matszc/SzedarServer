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
        public Tournament GetTournament(Guid id)
        {
            return _context.Tournaments.Where(t => t.Id == id)
                .Include(b => b.Players)
                .ThenInclude(x => x.Results)
                .Include(b => b.Matches)
                .ThenInclude(m => m.Result)
                .FirstOrDefault();
            // return await _context.Tournaments.SingleOrDefaultAsync(t => t.Id == id);
        }

        public IEnumerable<Tournament> GetAllUserTournaments(Guid userId)
        {
            return _context.Tournaments.Where(t => t.UserId == userId);
        }
    }
}