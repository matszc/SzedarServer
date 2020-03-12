using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using szedarserver.Core.Domain;
using szedarserver.Core.Domain.Context;
using szedarserver.Core.IRepositories;

namespace szedarserver.Core.Repositories
{
    public class TournamentRepository: ITournamentRepository
    {
        private readonly DataBaseContext _context;

        public TournamentRepository(DataBaseContext context)
        {
            _context = context;
        }
        
        public async Task<Tournament> CreateTournamentAsync(Tournament tournament, IEnumerable<Player> players,
            IEnumerable<Match> matches, IEnumerable<Result> results)
        {
            _context.Tournaments.Add(tournament);
            foreach (var player in players)
            {
                _context.Players.Add(player);
            }

            foreach (var match in matches)
            {
                _context.Matches.Add(match);
            }

            foreach (var result in results)
            {
                _context.Results.Add(result);
            }

            await _context.SaveChangesAsync();
            return tournament;
        }
        public Tournament GetTournament(Guid id)
        {
            return _context.Tournaments.Where(t => t.Id == id)
                .Include(b => b.Players)
                .ThenInclude(x => x.Results)
                .Include(b => b.Matches)
                .ThenInclude(m => m.Result)
                .FirstOrDefault();
        }

        public IEnumerable<Tournament> GetAllUserTournaments(Guid userId)
        {
            return _context.Tournaments.Where(t => t.UserId == userId)
                .Include(b => b.Players);
        }

        public Match GetMatch(Guid id)
        {
            return _context.Matches.Where(t => t.Id == id)
                .Include(b => b.Result)
                .ThenInclude(r => r.Player)
                .SingleOrDefault();
        }

        public Tournament GetRawTournament(Guid id)
        {
            return _context.Tournaments.SingleOrDefault(t => t.Id == id);
        }

        public async Task UpdateResultAsync(Result result1, Result result2)
        {
            _context.Results.Update(result1);
            _context.Results.Update(result2);
            
            await _context.SaveChangesAsync();
        }

        public async Task AddResultAsync(Result result)
        {
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteResultAsync(Result result)
        {
            _context.Results.Remove(result);
            await _context.SaveChangesAsync();
        }

        public async Task AddResultsAsync(IEnumerable<Result> results)
        {
            foreach (var result in results)
            {
                _context.Results.Add(result);
            }

            await _context.SaveChangesAsync();
        }
        public async Task AddOpenTournamentWithPlayersAsync(Tournament tournament, IEnumerable<Player> players)
        {
            _context.Tournaments.Add(tournament);
            foreach (var player in players)
            {
                _context.Players.Add(player);
            }

            await _context.SaveChangesAsync();
        }

        public Tournament GetTournamentWithPlayers(Guid tournamentId)
        {
            return _context.Tournaments.Where(t => t.Id == tournamentId)
                .Include(p => p.Players).SingleOrDefault();
        }

        public async Task StartTournamentAsync(Tournament tournament, IEnumerable<Match> matches, IEnumerable<Result> results)
        {
            tournament.Open = false;
            
            foreach (var match in matches)
            {
                _context.Add(match);
            }

            foreach (var result in results)
            {
                _context.Results.Add(result);
            }

            _context.Tournaments.Update(tournament);
            
            await _context.SaveChangesAsync();
        }

        public async Task AddPlayersToTournamentAsync(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                _context.Players.Add(player);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemovePlayerAsync(Guid playerId, Guid tournamentId)
        {
            _context.Players.Remove(_context.Players.Single(p => p.Id == playerId && p.TournamentId == tournamentId));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOpenTournamentAsync(Guid tournamentId, Tournament tournament)
        {
            var t = await _context.Tournaments.SingleAsync(t => t.Id == tournamentId);
            
            t.Address = tournament.Address;
            t.City = tournament.City;
            t.GameType = tournament.GameType;
            t.StartDate = tournament.StartDate;
            
            await _context.SaveChangesAsync();
        }

        public async Task CloseOpenTournamentAsync(Guid id)
        {
            _context.Tournaments.Remove(_context.Tournaments.Single(i => i.Id == id));
            var players = _context.Players.Where(i => i.TournamentId == id).ToList();

            foreach (var player in players)
            {
                _context.Players.Remove(player);
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task EditPlayerAsync(Guid id, string nick)
        {
            var player = _context.Players.Single(p => p.Id == id);
            player.Nick = nick;

            await _context.SaveChangesAsync();
        }
    }
}