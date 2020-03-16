using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.IServices
{
    public interface ISingleEliminationService
    {
        Task<Tournament> CreateSingleEliminationTournamentAsync(RegisterTournamentModel tournamentData,  Guid userId);
        IEnumerable<NodeDTO> GetFlatStructure (Guid tournamentId);
        Task StartTournamentAsync(Tournament tournament);
    }
}