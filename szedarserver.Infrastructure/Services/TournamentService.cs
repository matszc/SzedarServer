using System.Threading.Tasks;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;
using System;
using System.Collections.Generic;
using AutoMapper;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;

namespace szedarserver.Infrastructure.Services
{
    public class TournamentService: ITournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IMapper _mapper;

        public TournamentService(ITournamentRepository tournamentRepository, IMapper mapper)
        {
            _tournamentRepository = tournamentRepository;
            _mapper = mapper;
        }
        public async Task CreateSingleEliminationTournament(RegisterTournamentModel tournament, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task CreateDoubleEliminationTournament(RegisterTournamentModel tournament, Guid userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TournamentDTO> GetAllUserTournaments(Guid userId)
        {
            var tournaments = _tournamentRepository.GetAllUserTournaments(userId);
            var res = new List<TournamentDTO>();

            foreach (var tournament in tournaments)
            {
                res.Add(_mapper.Map<TournamentDTO>(tournament));
            }

            return res;
        }
    }
}