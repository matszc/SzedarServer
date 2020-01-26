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
        Task UpdateResult(Result result1, Result result2);

        Task AddResult(Result result);

        Task DeleteResult(Result result);

        Task AddResultsAsync(IEnumerable<Result> results);

    }
}