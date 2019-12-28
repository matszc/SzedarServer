using System;
using System.Threading.Tasks;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.Services
{
    public class SingleEliminationService: ISingleEliminationService
    {
        public Task CreateSingleEliminationTournament(RegisterTournamentModel tournament, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}