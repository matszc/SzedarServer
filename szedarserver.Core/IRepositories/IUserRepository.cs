using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using szedarserver.Core.Domain;

namespace szedarserver.Core.IRepositories
{
    public interface IUserRepository
    {
        public Task AddUserAsync(User user);
        public Task<User> GetByEmailAsync(string email);
        public Task<User> GetByLoginAsync(string login);
        public Task<User> GetUserAsync(User user);
    }
}
