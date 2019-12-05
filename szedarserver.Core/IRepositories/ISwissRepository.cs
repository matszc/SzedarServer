using System.Collections.Generic;
using System.Threading.Tasks;
using szedarserver.Core.Domain;

namespace szedarserver.Core.IRepositories
{
    public interface ISwissRepository
    {
        Task<Tournament> CreateTournamentAsync(Tournament tournament, IEnumerable<Player> players, IEnumerable<Match> matches, IEnumerable<Result> results);
/*        Task AddPlayersAsync(IEnumerable<Player> players);

        Task AddMatches(IEnumerable<Match> matches);

        Task AddResults(IEnumerable<Result> results);*/
    }
}