using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.IServices
{
    public interface IUserService
    {
        public Task RegisterAsync(UserRegisterModel user);
        public Task<AccountDTO> LoginAsync(LoginModel user);
    }
}
