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
        Task<Tournament> CreateDoubleEliminationAsync(RegisterTournamentModel form, Guid userId);
        IEnumerable<NodeDTO> GetMatches(Guid tournamentId);
        Task AddResultAsync(Guid matchId, MatchDTO match);
        Task StartTournamentAsync(Tournament tournament);
    }
}