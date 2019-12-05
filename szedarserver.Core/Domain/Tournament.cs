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
        public Guid UserId { get; set; }
        public TournamentsTypes Type { get; set; }
        public DateTime CreationDate { get; private set; }
        public int NumberOfRounds { get; private set; }
        
        public int CurrentRound { get; set; }

        public virtual IEnumerable<Player> Players { get; set; }
        public virtual IEnumerable<Match> Matches { get; set; }

        public Tournament(){}

        public Tournament(string name, int numberOfRounds, Guid userId, TournamentsTypes type)
        {
            UserId = userId;
            NumberOfRounds = numberOfRounds;
            Name = name;
            CreationDate = DateTime.UtcNow;
            Type = type;
            CurrentRound = 1;
        }
    }
    
    public enum TournamentsTypes
    {
        DoubleElimination = 0,
        SingleElimination = 1,
        Siwss = 2
    }
}