using System;
using System.Collections;
using System.Collections.Generic;
using szedarserver.Core.Domain;

namespace szedarserver.Infrastructure.DTO
{
    public class OpenTournamentDTO
    {
        public Guid Id { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime StartDate { get; set; }
        public GameTypes GameType { get; set; }
        public IEnumerable<PlayerDTO> Players { get; set; }
    }
}