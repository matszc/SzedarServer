using szedarserver.Core.Domain.Context;

namespace szedarserver.Core.Repositories
{
    public class TournamentRepository
    {
        private readonly DataBaseContext _context;

        public TournamentRepository(DataBaseContext context)
        {
            _context = context;
        }

    }
}