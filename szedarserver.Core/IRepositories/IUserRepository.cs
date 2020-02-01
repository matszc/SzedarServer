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
        Task<User> GetByLoginAsync(string login);
        Task<User> GetUserAsync(User user);
        User GetUserById(Guid id);
        Task<User> GetUserByFbIdAsync(string fbId);
        Task<Guid> GetUserIdByMatchId(Guid id);
        IEnumerable<Tournament> GetAllTournaments(Guid userId, GameTypes gameType);
        Task AddPlayerToTournament(Player user);
    }
}
