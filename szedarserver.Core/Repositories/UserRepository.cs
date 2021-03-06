﻿using AutoMapper;
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

        public User GetByLogin(string login)
        {
            try
            {
                return _context.Users.SingleOrDefault(x => x.Login == login);
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

        public User GetUserById(Guid id)
        {
            return _context.Users.SingleOrDefault(u => u.Id == id);
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

        public async Task<Guid> GetUserIdByMatchIdAsync(Guid id)
        {
            var match =  await _context.Matches.SingleOrDefaultAsync(m => m.Id == id);
            var tournament = await _context.Tournaments.SingleAsync(t => t.Id == match.TournamentId);
            return tournament.UserId;
        }

        public IEnumerable<Tournament> GetAllTournaments(Guid userId, GameTypes gameType)
        {
            if (gameType == GameTypes.All)
            {
                return _context.Tournaments.Where(t => t.UserId != userId && t.Open)
                    .Include(p => p.Players).ToList();
            }

            return _context.Tournaments.Where(t => t.UserId != userId && t.Open && t.GameType == gameType)
                .Include(p => p.Players).ToList();
        }

        public async Task AddPlayerToTournamentAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        }

        public List<Player> GetAllUserPlayers(IEnumerable<Guid> tournamentsIds)
        {
            return _context.Players.Where(p => tournamentsIds.Contains(p.TournamentId))
                .Include(r => r.Results)
                .ThenInclude(m => m.Match)
                .ThenInclude(i => i.Result).ToList();
        }

        public List<Tournament> GetAllUserTournaments(Guid userId)
        {
            return _context.Players.Where(p => p.UserId == userId)
                .Include(t => t.Tournament).Select(i => i.Tournament).ToList();
        }

        public List<Player> GetAllPlayerByUserId(Guid userId)
        {
            return _context.Players.Where(p => p.UserId == userId)
                .Include(l => l.Results)
                .ThenInclude(r => r.Match)
                .ThenInclude(m => m.Result)
                .ThenInclude(s => s.Player).ToList();
        }

        public async Task DeletePlayerAsync(Guid tournamentId, Guid userId)
        {
            var p = await _context.Players.SingleAsync(p => p.UserId == userId && p.TournamentId == tournamentId);
            _context.Players.Remove(p);
            await _context.SaveChangesAsync();

        }
    }
}
