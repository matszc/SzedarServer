using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using szedarserver.Core.Domain;

namespace szedarserver.Core.IRepositories
{
    public interface ITreeRepository
    {
        Match GetMatchByCode(string matchCode, Guid tournamentId);
        List<Match> GetAllMatches(Guid tournamentId);
    }
}