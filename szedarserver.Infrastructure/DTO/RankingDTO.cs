using System;

namespace szedarserver.Infrastructure.DTO
{
    public class RankingDTO
    {
        public Guid Id { get; set; }
        public string Player { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }
    }
}