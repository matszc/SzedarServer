using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Core.Domain.Context;
using szedarserver.Core.IRepositories;

namespace szedarserver.Core.Repositories
{
    public class TreeRepository: ITreeRepository
    {
        private readonly DataBaseContext _context;
        
        public TreeRepository (DataBaseContext context)
        {
            _context = context;
        }
    }
}