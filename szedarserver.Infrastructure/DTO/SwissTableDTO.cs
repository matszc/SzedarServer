using szedarserver.Core.Domain;

namespace szedarserver.Infrastructure.DTO
{
    public class SwissTableDTO
    {
        public string Player { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }
    }
}