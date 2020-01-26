using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.IServices
{
    public interface IDoubleEliminationService
    {
        Task<Tournament> CreateDoubleElimination(RegisterTournamentModel form, Guid userId);
        IEnumerable<NodeDTO> GetMatches(Guid tournamentId);
        Task AddResult(Guid matchId, MatchDTO match);
    }
}