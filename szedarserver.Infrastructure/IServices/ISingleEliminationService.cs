using System;
using System.Threading.Tasks;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.IServices
{
    public interface ISingleEliminationService
    {
        Task CreateSingleEliminationTournament(RegisterTournamentModel tournament,  Guid userId);
    }
}