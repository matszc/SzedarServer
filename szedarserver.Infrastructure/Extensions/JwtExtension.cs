using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.Setting;

namespace szedarserver.Infrastructure.Extensions
{
    public class JwtExtension : IJwtExtension
    {
        private readonly JwtSettings _jwtSettings;

        public JwtExtension(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string CreateToken(AccountDTO user)
        {
            var now = DateTime.UtcNow;
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256 
                );
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.Iat, now.ToString())
            };
            var jwt = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                claims: claims,
                notBefore: now,
                expires: now.AddDays(_jwtSettings.ExpireDays),
                signingCredentials: signingCredentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;
        }
    }
}