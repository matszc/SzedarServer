using System.Collections.Generic;
using szedarserver.Core.Domain;

namespace szedarserver.Infrastructure.Models
{
    public class TournamentParts
    {
        public Tournament Tournament { get; set; }
        public IEnumerable<Player> Players { get; set; }
        public IEnumerable<Match> Matches { get; set; }
        public IEnumerable<Result> Results { get; set; }
    }
}