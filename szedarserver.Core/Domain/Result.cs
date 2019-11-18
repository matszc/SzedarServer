namespace szedarserver.Core.Domain
{
    public class Result: EntityGuid
    {
        public Player Player { get; set; }
        public Match Match { get; set; }
        public int Score { get; set; }
        public bool Win { get; set; }
    }
}