using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.IServices
{
    public interface ITournamentService
    {
        IEnumerable<TournamentDTO> GetAllUserTournaments(Guid userId);

        TournamentParts CreateUpperTree(RegisterTournamentModel tournamentData,
            Guid userId);
    }
}