using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using szedarserver.Core.Domain;

namespace szedarserver.Core.IRepositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User> GetByEmailAsync(string email);
        User GetByLogin(string login);
        Task<User> GetUserAsync(User user);
        User GetUserById(Guid id);
        Task<User> GetUserByFbIdAsync(string fbId);
        Task<Guid> GetUserIdByMatchIdAsync(Guid id);
        IEnumerable<Tournament> GetAllTournaments(Guid userId, GameTypes gameType);
        Task AddPlayerToTournamentAsync(Player user);
        List<Player> GetAllUserPlayers(IEnumerable<Guid> userIds);
        List<Tournament> GetAllUserTournaments(Guid userId);
        List<Player> GetAllPlayerByUserId(Guid userId);
        Task DeletePlayerAsync(Guid tournamentId, Guid userId);
    }
}
