using System.Threading.Tasks;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;
using System;

namespace szedarserver.Infrastructure.Services
{
    public class TournamentService: ITournamentService
    {
        public async Task CreateSingleEliminationTournament(RegisterTournamentModel tournament, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task CreateDoubleEliminationTournament(RegisterTournamentModel tournament, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task CreateSwissTournament(RegisterTournamentModel tournament, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}