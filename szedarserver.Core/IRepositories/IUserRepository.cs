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
        Task<Guid> GetUserIdByMatchId(Guid id);
        IEnumerable<Tournament> GetAllTournaments(Guid userId, GameTypes gameType);
        Task AddPlayerToTournament(Player user);
        List<Player> GetAllUserPlayers(IEnumerable<Guid> userIds);
        List<Tournament> GetAllUserTournaments(Guid userId);
        List<Player> GetAllPlayerByUserId(Guid userId);
        Task DeletePlayer(Guid tournamentId, Guid userId);
    }
}
