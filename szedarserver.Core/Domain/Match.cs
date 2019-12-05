using System;
using System.Collections.Generic;

namespace szedarserver.Core.Domain
{
    public class Match: EntityGuid
    {
        public string MatchCode { get; set; }
        public string NextMachCode { get; set; }
        public int Round { get; set; }
        public virtual IEnumerable<Result> Result { get; set; }
        public virtual Tournament Tournament { get; set; }
        public Guid TournamentId { get; set; }

        public Match(int round, Guid tournamentId)
        {
            Id = Guid.NewGuid();
            Round = round;
            TournamentId = tournamentId;
        }
    }
}