using System.Collections.Generic;

namespace szedarserver.Infrastructure.DTO
{
    public class SwissDTO
    {
        public SwissTableDTO[] SwissTable { get; set; }
        public  IEnumerable<RoundDTO> Rounds  { get; set; }
    }
}