using System.Collections.Generic;
using szedarserver.Core.Domain;

namespace szedarserver.Infrastructure.Models
{
    public class SwissRoundModel
    {
        public IEnumerable<Match> Matches { get; set; }
        public IEnumerable<Result> Results { get; set; }

        public SwissRoundModel(IEnumerable<Match> matches, IEnumerable<Result> results)
        {
            Matches = matches;
            Results = results;
        }
    }
}