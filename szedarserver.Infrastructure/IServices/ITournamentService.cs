using System;
using System.Threading.Tasks;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.IServices
{
    public interface ITournamentService
    {
        Task CreateSingleEliminationTournament(RegisterTournamentModel tournament,  Guid userId);
        Task CreateDoubleEliminationTournament(RegisterTournamentModel tournament, Guid userId);
    }
}