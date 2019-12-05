using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using szedarserver.Core.Domain;

namespace szedarserver.Core.IRepositories
{
    public interface ITournamentRepository
    {
        Tournament GetTournament(Guid id);

        IEnumerable<Tournament> GetAllUserTournaments(Guid userId);
    }
}