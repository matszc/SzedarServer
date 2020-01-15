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
        Task<Tournament> CreateSingleEliminationTournament(RegisterTournamentModel tournamentData,  Guid userId);
        IEnumerable<NodeDTO> GetSingleEliminationTree(Guid tournamentId);
        IEnumerable<NodeDTO> GetFlatStructure (Guid tournamentId);
        Task UpdateResult(MatchDTO matchDto);
    }
}