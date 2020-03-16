using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.Models;
using Match = szedarserver.Core.Domain.Match;

namespace szedarserver.Infrastructure.IServices
{
    public interface ITournamentService
    {
        IEnumerable<TournamentDTO> GetAllUserTournaments(Guid userId);

        TournamentParts CreateUpperTree(RegisterTournamentModel tournamentData,
            Guid userId);

        IEnumerable<NodeDTO> GetFlatStructure(Tournament tournament);

        TournamentParts StartUpperTree(Tournament tournament);

        Task UpdateSingleEliminationTreeAsync(MatchDTO match);
        Task CreateOpenTournamentAsync(RegisterTournamentModel tournament, Guid userId);
        Task AddPlayersAsync(string[] nicks, Guid tournamentId);
        OpenTournamentDTO GetOpenTorTournament(Guid tournamentId);
        Task UpdateOpenTournamentAsync(Guid tournamentId, OpenTournamentDTO tournamentDto);
    }
}