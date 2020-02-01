using System;
using szedarserver.Core.Domain;

namespace szedarserver.Infrastructure.DTO
{
    public class TournamentDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public TournamentsTypes Type { get; set; }
        public int NumberOfRounds { get; set; }
        public DateTime StartDate { get; set; }
        public GameTypes GameType { get; set; }
        public int MaxNumberOfPlayers { get; set; }
        public int NumberOfPlayers { get; set; }
        public bool Open { get; set; }
        
    }
}