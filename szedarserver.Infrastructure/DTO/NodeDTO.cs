using System;

namespace szedarserver.Infrastructure.DTO
{
    public class NodeDTO : MatchDTO
    { 
        public  string MatchCode { get; set; }
        public int Round { get; set; }
        
        public NodeDTO NextMatch { get; set; }
    }
}