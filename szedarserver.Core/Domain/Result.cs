using System;

namespace szedarserver.Core.Domain
{
    public class Result: EntityGuid
    {
        public virtual Player Player { get; set; }
        public virtual Match Match { get; set; }

        public Guid PlayerId { get; set; }
        public Guid MatchId { get; set; }
        public bool Win { get; set; }
        public int Score { get; set; }

        public Result() { }

        public Result(Guid playerId, Guid matchId)
        {
            PlayerId = playerId;
            MatchId = matchId;
            Score = 0;
            Win = false;
        }
    }
}