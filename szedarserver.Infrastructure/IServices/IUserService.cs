using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.IServices
{
    public interface IUserService
    {
        Task RegisterAsync(UserRegisterModel user);
        Task<AccountDTO> LoginAsync(LoginModel user);

        Task<AccountDTO> LoginFbAsync(FbUserModel user);
        IEnumerable<TournamentDTO> GetAllAvailableTournaments(Guid userId, GameTypes gameType);
        Task JoinTournamentAsync(Guid userId, Tournament tournament);
        IEnumerable<RankingDTO> GetPlayersRanking(Guid userId);
        ProfileDTO GetUserProfile(User user);
    }
}
