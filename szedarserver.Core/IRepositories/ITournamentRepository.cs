using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using szedarserver.Core.Domain;

namespace szedarserver.Core.IRepositories
{
    public interface ITournamentRepository
    {
        Task<Tournament> CreateTournamentAsync(Tournament tournament, IEnumerable<Player> players, IEnumerable<Match> matches, IEnumerable<Result> results);
        Tournament GetTournament(Guid id);

        IEnumerable<Tournament> GetAllUserTournaments(Guid userId);
        Match GetMatch(Guid id);

        Tournament GetRawTournament(Guid id);
        Task UpdateResultAsync(Result result1, Result result2);

        Task AddResultAsync(Result result);

        Task DeleteResultAsync(Result result);

        Task AddResultsAsync(IEnumerable<Result> results);
        Task AddOpenTournamentWithPlayersAsync(Tournament tournament, IEnumerable<Player> players);

        Tournament GetTournamentWithPlayers(Guid tournamentId);
        Task StartTournamentAsync(Tournament tournament, IEnumerable<Match> matches, IEnumerable<Result> results);

        Task AddPlayersToTournamentAsync(IEnumerable<Player> players);
        Task RemovePlayerAsync(Guid playerId, Guid tournamentId);

        Task UpdateOpenTournamentAsync(Guid tournamentId, Tournament tournament);

    }
}