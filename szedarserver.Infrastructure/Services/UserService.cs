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
        private readonly IJwtExtension _jwtExtension;
        public UserService(IUserRepository userRepository, IMapper mapper, IJwtExtension jwtExtension)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtExtension = jwtExtension;
        }

        public async Task<AccountDTO> LoginAsync(LoginModel user)
        {
            user.Password = HashExtension.HashPassword(user.Password);
            var User = await _userRepository.GetUserAsync(_mapper.Map<User>(user));
            if(User == null)
            {
                return null;
            }
            var account = _mapper.Map<AccountDTO>(User);

            account.Token = _jwtExtension.CreateToken(account);

            return account;

        }

        public async Task<AccountDTO> LoginFbAsync(FbUserModel user)
        {
            var userFromDb = await _userRepository.GetUserByFbIdAsync(user.FbId);
            if (userFromDb == null)
            {
                userFromDb = new User(user.Email, user.Login, user.FbId, "");
                await _userRepository.AddUserAsync(userFromDb);
            }
            var account = _mapper.Map<AccountDTO>(userFromDb);
            account.Token = _jwtExtension.CreateToken(account);
            return account;
        }

        public async Task RegisterAsync(UserRegisterModel user)
        {
            var userFromDbByLogin = await _userRepository.GetByEmailAsync(user.Email);
            if(userFromDbByLogin != null)
            {
                throw new ValidationException("Login already exists");
            }
            var userFromDbByEmail = await _userRepository.GetByLoginAsync(user.Login);
            if(userFromDbByEmail != null)
            {
                throw new ValidationException("Email already exists");
            }

            if (user.Password.Length <= 4)
            {
                throw new ValidationException("Password too short");
            }
            var newUser = new User(user.Email, HashExtension.HashPassword(user.Password), user.Login);
            await _userRepository.AddUserAsync(newUser);

        }

    }
}
