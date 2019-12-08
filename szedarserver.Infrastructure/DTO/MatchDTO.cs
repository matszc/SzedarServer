using System;

namespace szedarserver.Infrastructure.DTO
{
    public class MatchDTO
    {
        public Guid Id { get; set; }
        public string Player1 { get; set; }
        public int Player1Score { get; set; }
        public string Player2 { get; set; }
        public int Player2Score { get; set; }
    }

    public class PlayerMatchDTO
    {
        public string Nick { get; set; }
        public bool Win { get; set; }
    }
}