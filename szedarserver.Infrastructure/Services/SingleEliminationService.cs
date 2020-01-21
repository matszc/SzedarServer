using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;

namespace szedarserver.Infrastructure.Services
{
    public class SingleEliminationService : ISingleEliminationService
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITreeRepository _treeRepository;
        private readonly ITournamentService _tournamentService;

        public SingleEliminationService(ITournamentRepository tournamentRepository, ITreeRepository treeRepository,
            ITournamentService tournamentService)
        {
            _tournamentRepository = tournamentRepository;
            _treeRepository = treeRepository;
            _tournamentService = tournamentService;
        }

        public async Task<Tournament> CreateSingleEliminationTournament(RegisterTournamentModel tournamentData,
            Guid userId)
        {

            var upperTree = _tournamentService.CreateUpperTree(tournamentData, userId);

            await _tournamentRepository.CreateTournamentAsync(upperTree.Tournament, upperTree.Players, upperTree.Matches, 
                upperTree.Results);

            return upperTree.Tournament;
        }

        public IEnumerable<NodeDTO> GetSingleEliminationTree(Guid tournamentId)
        {
            var tournament = _tournamentRepository.GetTournament(tournamentId);

            if (tournament == null)
            {
                return null;
            }

            return GetNodes(tournament);
        }

        public IEnumerable<NodeDTO> GetFlatStructure(Guid tournamentId)
        {
            var tournament = _tournamentRepository.GetTournament(tournamentId);

            if (tournament == null)
            {
                return null;
            }

            return GetFlatStructure(tournament).OrderBy(t => t.MatchCode);
        }

        public async Task UpdateResult(MatchDTO matchDto)
        {
            var match = _tournamentRepository.GetMatch(matchDto.Id);
            
            var nextMatch = _treeRepository.GetMatchByCode(match.NextMachCode, match.TournamentId);
            
            if (match.Result.Count() != 2 || matchDto.Player1Score == matchDto.Player2Score
                || (nextMatch.Result != null && nextMatch.Result.SingleOrDefault(r => r.Win) != null))
            {
                throw new ValidationException("Can't update this match");
            }
            var p1 = match.Result.Single(r => r.Player.Nick == matchDto.Player1);
            var p2 = match.Result.Single(r => r.Player.Nick == matchDto.Player2);
            
            p1.Score = matchDto.Player1Score;
            p1.Win = matchDto.Player1Score > matchDto.Player2Score;
            
            p2.Score = matchDto.Player2Score;
            p2.Win = matchDto.Player1Score < matchDto.Player2Score;

            await _tournamentRepository.UpdateResult(p1, p2);

            var winner = matchDto.Player1Score > matchDto.Player2Score ? p1 : p2;

            if (nextMatch.Result != null && nextMatch.Result.SingleOrDefault(r => r.PlayerId == p1.PlayerId || r.PlayerId == p2.PlayerId) != null)
            {
                await _tournamentRepository.DeleteResult(nextMatch.Result.Single(r => r.PlayerId == p1.PlayerId || r.PlayerId == p2.PlayerId));
            }
            
            var result = new Result(winner.PlayerId, nextMatch.Id){};

            await _tournamentRepository.AddResult(result);
        }

        private IEnumerable<NodeDTO> GetNodes(Tournament tournament)
        {
            var firsRound = tournament.Matches.Where(m => m.Round == 1);
            var res = new List<NodeDTO>();
            foreach (var match in firsRound)
            {
                var node = new NodeDTO()
                {
                    Id = match.Id,
                    MatchCode = match.MatchCode,
                    Round = match.Round,
                    EditAble = match.EditAble,
                    NextMatch = CreateNode(tournament, match.NextMachCode, false, true),
                };
                if (match.Result.Any())
                {
                    node.Player1 = match.Result.First().Player.Nick;
                    node.Player1Score = match.Result.First().Score;
                }
                if (match.Result.Count() > 1)
                {
                    node.Player2Score = match.Result.Last().Score;
                    node.Player2 = match.Result.Last().Player.Nick;
                }

                res.Add(node);
            }

            return res;
        }

        private IEnumerable<NodeDTO> GetFlatStructure(Tournament tournament)
        {
            var res = new List<NodeDTO>();
            foreach (var match in tournament.Matches)
            {
                var node = new NodeDTO()
                {
                    Id = match.Id,
                    MatchCode = match.MatchCode,
                    Round = match.Round,
                    EditAble = match.Result.Count() == 2,
                    NextMatch = match.NextMachCode != null? CreateNode(tournament, match.NextMachCode, false, true): null,
                };
                if (match.Result.Any())
                {
                    node.Player1 = match.Result.First().Player.Nick;
                    node.Player1Score = match.Result.First().Score;
                }
                if (match.Result.Count() > 1)
                {
                    node.Player2Score = match.Result.Last().Score;
                    node.Player2 = match.Result.Last().Player.Nick;
                }

                res.Add(node);
            }

            return res;
        }

        private NodeDTO CreateNode(Tournament tournament, string matchCode, bool flat, bool recursionFlag)
        {
            if (!recursionFlag)
            {
                return null;
            }
            
            var match = tournament.Matches.First(m => m.MatchCode == matchCode);
            if (match.NextMachCode != null)
            {
                var p1 = match.Result.FirstOrDefault()?.Player.Nick;
                var p1Score = match.Result.FirstOrDefault()?.Score;
                var p2 = match.Result.LastOrDefault()?.Player.Nick;
                var p2Score = match.Result.LastOrDefault()?.Score;
                
                return new NodeDTO()
                {
                    MatchCode = match.MatchCode,
                    Id = match.Id,
                    Round = match.Round,
                    Player1 = p1,
                    Player1Score = p1Score != null? p1Score.Value: 0,
                    Player2 = p1 != p2? p2: null,
                    Player2Score = p1 != p2? p2Score != null? p2Score.Value :0: 0,
                    EditAble = match.EditAble,
                    NextMatch = CreateNode(tournament, match.NextMachCode, flat, flat)
                };
            }

            var lastNode = new NodeDTO()
            {
                MatchCode = match.MatchCode,
                Id = match.Id,
                Round = match.Round,
                EditAble = match.EditAble,
            };
            
            if (match.Result.Any())
            {
                lastNode.Player1 = match.Result.First().Player.Nick;
                lastNode.Player1Score = match.Result.First().Score;
            }

            if (match.Result.Count() > 1)
            {
                lastNode.Player2Score = match.Result.Last().Score;
                lastNode.Player2 = match.Result.Last().Player.Nick;
            }

            return lastNode;
        }
        
    }
}