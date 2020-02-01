using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.Services
{
    public class SingleEliminationService : ISingleEliminationService
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITreeRepository _treeRepository;
        private readonly ITournamentService _tournamentService;

        public SingleEliminationService(ITournamentRepository tournamentRepository, ITreeRepository treeRepository,
            ITournamentService tournamentService)
        {
            _tournamentRepository = tournamentRepository;
            _treeRepository = treeRepository;
            _tournamentService = tournamentService;
        }

        public async Task<Tournament> CreateSingleEliminationTournament(RegisterTournamentModel tournamentData,
            Guid userId)
        {

            var upperTree = _tournamentService.CreateUpperTree(tournamentData, userId);

            await _tournamentRepository.CreateTournamentAsync(upperTree.Tournament, upperTree.Players, upperTree.Matches, 
                upperTree.Results);

            return upperTree.Tournament;
        }
        public IEnumerable<NodeDTO> GetFlatStructure(Guid tournamentId)
        {
            var tournament = _tournamentRepository.GetTournament(tournamentId);

            if (tournament == null)
            {
                return null;
            }

            return _tournamentService.GetFlatStructure(tournament).OrderBy(t => t.MatchCode);
        }

        public async Task StartTournament(Tournament tournament)
        {
            var tournamentParts = _tournamentService.StartUpperTree(tournament);
            await _tournamentRepository.StartTournament(tournament, tournamentParts.Matches, tournamentParts.Results);
        }
    }
}