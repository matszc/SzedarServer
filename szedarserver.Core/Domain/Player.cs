using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace szedarserver.Core.Domain
{
    public class Player: EntityGuid
    {
        public string Nick { get; set; }
        public virtual IEnumerable<Result> Results { get; set; }
        public Tournament Tournament { get; set; }


        public Player() { }

        public Player(string nick, Tournament tournament)
        {
            Nick = nick;
            Tournament = tournament;
            Id = Guid.NewGuid();
        }
    }
}