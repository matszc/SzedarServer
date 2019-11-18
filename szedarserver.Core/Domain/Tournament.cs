using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace szedarserver.Core.Domain
{
    public class Tournament: EntityGuid
    {
        [Required]
        public virtual User User { get; set; }
        [Required]
        public string Name { get; private set; }
        public DateTime CreationDate { get; private set; }

        public virtual IEnumerable<Player> Players { get; set; }
        public virtual IEnumerable<Match> Matches { get; set; }

        public Tournament(){}

        public Tournament(string name)
        {
            Name = name;
            CreationDate = DateTime.UtcNow;
        }
    }
}