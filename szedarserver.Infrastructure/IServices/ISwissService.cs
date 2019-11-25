using System;
using System.Threading.Tasks;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.IServices
{
    public interface ISwissService
    {
        Task CreateSwissTournament(RegisterTournamentModel tournament, Guid userId);
    }
}