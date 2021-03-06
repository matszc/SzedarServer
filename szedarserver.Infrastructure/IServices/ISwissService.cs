using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.IServices
{
    public interface ISwissService
    {
        Task<Tournament> CreateSwissTournamentAsync(RegisterTournamentModel tournament, Guid userId);
        SwissDTO GetTournamentData(Guid tournamentId);
        Task AddResultAsync(Guid matchId, MatchDTO match);

        Task MoveNextRoundAsync(Guid tournamentId);
        Task StartTournamentAsync(Tournament tournament);
    }
}