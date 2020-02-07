using System;
using System.Collections.Generic;
using szedarserver.Core.Domain;

namespace szedarserver.Infrastructure.DTO
{
    public class SwissTableDTO: RankingDTO
    {
        public int T1 { get; set; }
        public int T2 { get; set; }
        public int T3 { get; set; }
        public IEnumerable<Guid> oponentsIds { get; set; }
    }
    
}