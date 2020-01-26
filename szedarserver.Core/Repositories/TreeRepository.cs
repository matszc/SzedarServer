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
    public class TreeRepository: ITreeRepository
    {
        private readonly DataBaseContext _context;
        
        public TreeRepository (DataBaseContext context)
        {
            _context = context;
        }

        public Match GetMatchByCode(string matchCode, Guid tournamentId)
        {
            return _context.Matches.Where(m => m.TournamentId == tournamentId && m.MatchCode == matchCode)
                .Include(b => b.Result)
                .ThenInclude(r => r.Player)
                .SingleOrDefault();
        }

        public List<Match> GetAllMatches(Guid tournamentId)
        {
            return _context.Matches.Where(m => m.TournamentId == tournamentId)
                .Include(b => b.Result).ToList();
        }
    }
}