using System.Collections.Generic;

namespace szedarserver.Core.Domain
{
    public class Player: EntityGuid
    {
        public string Nick { get; set; }
        public IEnumerable<Result> Matches { get; set; }
        public Tournament Tournament { get; set; }
    }
}