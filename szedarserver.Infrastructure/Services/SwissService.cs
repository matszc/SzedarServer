using System;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.Services
{
    public class SwissService: ISwissService
    {
        public async Task CreateSwissTournament(RegisterTournamentModel tournament, Guid userId)
        {
            var newTournament = new Tournament(tournament.Name, tournament.Rounds, userId);
            
        }
    }
}