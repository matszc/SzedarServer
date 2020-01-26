using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Core.Domain.Context;
using szedarserver.Core.IRepositories;

namespace szedarserver.Core.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly DataBaseContext _context;
        public UserRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
            } catch(Exception e)
            {
                throw e;
            }
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                await Task.CompletedTask;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<User> GetByLoginAsync(string login)
        {
            try
            {
                return await _context.Users.SingleOrDefaultAsync(x => x.Login == login);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<User> GetUserAsync(User user)
        {
            try
            {
                return await _context.Users.SingleOrDefaultAsync(x => (x.Login == user.Login || x.Email == user.Login) && x.Password == user.Password);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<User> GetUserByFbIdAsync(string fbId)
        {
            try
            {
                return await _context.Users.SingleOrDefaultAsync(x => x.FbId == fbId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Guid> GetUserIdByMatchId(Guid id)
        {
            var match =  await _context.Matches.SingleOrDefaultAsync(m => m.Id == id);
            var tournament = await _context.Tournaments.SingleAsync(t => t.Id == match.TournamentId);
            return tournament.UserId;
        }
    }
}
