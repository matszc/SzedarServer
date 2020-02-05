using System;
using System.Collections.Generic;

namespace szedarserver.Infrastructure.DTO
{
    public class ProfileDTO
    {
        public Guid UserId { get; set; }
        public string Nick { get; set; }
        public List<TournamentDTO> UpComingTournament { get; set; }
        public List<TournamentDTO> PastTournaments { get; set; }
        public List<MatchDTO> PastMatches { get; set; }
    }
}