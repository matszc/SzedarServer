using System.Collections.Generic;

namespace szedarserver.Core.Domain
{
    public class Match: EntityGuid
    {
        public string MatchCode { get; set; }
        public string NextMachCode { get; set; }
        public int Round { get; set; }
        public string Result { get; set; }
        public IEnumerable<Result> Result1 { get; set; }
        public Tournament Tournament { get; set; }
    }
}