namespace szedarserver.Infrastructure.Models

{
    public class RegisterTournamentModel
    {
        public string Name { get; set; }
        public TournamentsTypes Type { get; set; }
        public string[] Players { get; set; }
        public decimal Rounds { get; set; }
    }
    public enum TournamentsTypes
    {
        DoubleElimination = 0,
        SingleElimination = 1,
        Siwss = 2
    }
}