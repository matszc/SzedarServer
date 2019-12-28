using System;
using System.Collections.Generic;
using szedarserver.Core.Domain;

namespace szedarserver.Infrastructure.DTO
{
    public class SwissTableDTO
    {
        public Guid Id { get; set; }
        public string Player { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Points { get; set; }
        public int T1 { get; set; }
        public int T2 { get; set; }
        public int T3 { get; set; }
        public int Position { get; set; }
        public IEnumerable<Guid> oponentsIds { get; set; }
    }
    
}