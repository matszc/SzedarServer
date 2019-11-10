using System;

namespace szedarserver.Infrastructure.Extensions
{
    public interface IJwtExtension
    {
        string CreateToken(Guid userId);
    }
}