using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.IServices
{
    public interface ITournamentService
    {
        Task CreateDoubleEliminationTournament(RegisterTournamentModel tournament, Guid userId);

        IEnumerable<TournamentDTO> GetAllUserTournaments(Guid userId);
    }
}