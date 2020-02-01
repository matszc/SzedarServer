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
        public int NumberOfRounds { get; private set; }
        public int CurrentRound { get; set; }
        public virtual IEnumerable<Player> Players { get; set; }
        public virtual IEnumerable<Match> Matches { get; set; }
        
        public int MaxNumberOfPlayers { get; set; }
        public GameTypes GameType { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        public DateTime CreationDate { get; protected set; } = DateTime.UtcNow;
        
        public DateTime StartDate { get; set; }
        public bool Open { get; set; }

        public Tournament(){}

        public Tournament(string name, int numberOfRounds, Guid userId, TournamentsTypes type)
        {
            UserId = userId;
            NumberOfRounds = numberOfRounds;
            Name = name;
            Type = type;
            CurrentRound = 1;
            Open = false;
        }
        public Tournament(string name, int numberOfRounds, Guid userId, TournamentsTypes type,
            int maxNumberOfPlayers, GameTypes gameType, string city,
            string address, DateTime startDate)
        {
            UserId = userId;
            NumberOfRounds = numberOfRounds;
            Name = name;
            Type = type;
            CurrentRound = 1;
            MaxNumberOfPlayers = maxNumberOfPlayers;
            GameType = gameType;
            City = city;
            Address = address;
            StartDate = startDate;
            Open = true;
        }

        public Tournament(string name, Guid userId, TournamentsTypes type)
        {
            UserId = userId;
            Name = name;
            Type = type;
            Open = false;
        }

        public Tournament(string name, Guid userId, TournamentsTypes type, int maxNumberOfPlayers, GameTypes gameType, string city,
            string address, DateTime startDate)
        {
            UserId = userId;
            Name = name;
            Type = type;
            MaxNumberOfPlayers = maxNumberOfPlayers;
            GameType = gameType;
            City = city;
            Address = address;
            StartDate = startDate;
            Open = true;
        }
    }
    
    public enum TournamentsTypes
    {
        DoubleElimination = 0,
        SingleElimination = 1,
        Siwss = 2
    }

    public enum GameTypes
    {
        All = 0,
        LeagueOfLegends = 1,
        Hearthstone = 2,
        CounterStrikeGlobalOffensive = 3,
        Dota2 = 4,
        Overwatch = 5,
        RocketLeague = 6,
        Fifa = 7,
        Other = 8,
    }
}