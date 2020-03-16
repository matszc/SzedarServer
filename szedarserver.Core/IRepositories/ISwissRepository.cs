using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using szedarserver.Core.Domain;

namespace szedarserver.Core.IRepositories
{
    public interface ISwissRepository
    {
        Task AddMatchesAsync(IEnumerable<Match> matches, IEnumerable<Result> results);

        Task MoveNextRoundAsync(Guid tournamentId, int currentRound);
    }
}