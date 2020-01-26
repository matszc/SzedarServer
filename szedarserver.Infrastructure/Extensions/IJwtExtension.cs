using System;
using szedarserver.Infrastructure.DTO;

namespace szedarserver.Infrastructure.Extensions
{
    public interface IJwtExtension
    {
        string CreateToken(AccountDTO user);
    }
}