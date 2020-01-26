using System.Collections.Generic;

namespace szedarserver.Infrastructure.DTO
{
    public class DoubleEliminationDTO
    {
        public IEnumerable<NodeDTO> UpperBracket { get; set; }
        public IEnumerable<NodeDTO> LowerBracket { get; set; }
    }
}