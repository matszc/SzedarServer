using System.Collections.Generic;

namespace szedarserver.Infrastructure.DTO
{
    public class RoundDTO
    {
        public IEnumerable<MatchDTO> MatchDtos { get; set; }
    }
}