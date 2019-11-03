using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.IServices;
using System.Security.Cryptography;
using szedarserver.Infrastructure.Extensions;
using szedarserver.Core.Domain;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using szedarserver.Infrastructure.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace szedarserver.Infrastructure.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<AccountDTO> LoginAsync(LoginModel user)
        {
            user.Password = HashExtension.HashPassword(user.Password);
            var User = await _userRepository.GetUserAsync(_mapper.Map<User>(user));
            if(User == null)
            {
                return null;
            }
            var accout = _mapper.Map<AccountDTO>(User);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("testbfdgdgtestbfdgdgtestbfdgdgtestbfdgdg");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, accout.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            accout.Token = tokenHandler.WriteToken(token);

            return accout;

        }

        public async Task RegisterAsync(UserRegisterModel _user)
        {
            var user = await _userRepository.GetByEmailAsync(_user.Email);
            if(user != null)
            {
                throw new ValidationException("Login already exists");
            }
            user = await _userRepository.GetByLoginAsync(_user.Login);
            if(user != null)
            {
                throw new ValidationException("Email already exists");
            }
            user = new User(_user.Email, HashExtension.HashPassword(_user.Password), _user.Login);
            await _userRepository.AddUserAsync(user);

        }

    }
}
