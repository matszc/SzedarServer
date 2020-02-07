using System;
using szedarserver.Core.Domain;

namespace szedarserver.Infrastructure.Models

{
    public class RegisterTournamentModel
    {
        public string Name { get; set; }
        public TournamentsTypes Type { get; set; }
        public string[] Players { get; set; }
        public int Rounds { get; set; }
        public int MaxNumberOfPlayers { get; set; }
        public GameTypes GameType { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public DateTime StartDate { get; set; }
    }
}