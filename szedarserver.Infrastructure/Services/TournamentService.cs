using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using szedarserver.Core.Domain;
using szedarserver.Core.IRepositories;
using szedarserver.Infrastructure.DTO;

namespace szedarserver.Infrastructure.Services
{
    public class TournamentService: ITournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IMapper _mapper;
        private readonly ITreeRepository _treeRepository;

        public TournamentService(ITournamentRepository tournamentRepository, IMapper mapper,
            ITreeRepository treeRepository)
        {
            _tournamentRepository = tournamentRepository;
            _mapper = mapper;
            _treeRepository = treeRepository;
        }

        public IEnumerable<TournamentDTO> GetAllUserTournaments(Guid userId)
        {
            var tournaments = _tournamentRepository.GetAllUserTournaments(userId);
            var res = new List<TournamentDTO>();

            foreach (var tournament in tournaments)
            {
                var item = _mapper.Map<TournamentDTO>(tournament);
                item.NumberOfPlayers = tournament.Players.Count();
                res.Add(item);
            }

            return res.OrderBy(r => r.CreationDate).Reverse();
        }

        public TournamentParts CreateUpperTree(RegisterTournamentModel tournamentData,
            Guid userId)
        {
            var tournament = new Tournament(tournamentData.Name, userId, tournamentData.Type);

            var matches = CreateMatches(tournamentData.Players.Length, tournament.Id);

            var players = new List<Player>();
            var results = new List<Result>();

            foreach (var player in tournamentData.Players)
            {
                if (players.Find(pl => pl.Nick == player) != null)
                {
                    continue;
                }
                var p = new Player(player, tournament);
                results.Add(new Result()
                {
                    PlayerId = p.Id,
                    Score = 0,
                    Win = false,
                });
                players.Add(p);
            }

            var firstRoundIds = matches.Where(m => m.Round == 1).Select(m => m.Id).ToList();

            foreach (var result in results)
            {
                var match = firstRoundIds.FirstOrDefault(m => !results.Where(r => m == r.MatchId).ToList().Any());

                if (match != Guid.Empty)
                {
                    result.MatchId = match;
                }
                else
                {
                    match = firstRoundIds.First(m => results.Where(r => m == r.MatchId).ToList().Count == 1);
                    result.MatchId = match;
                }
            }
            
            var newResults = new List<Result>();

            foreach (var result in results)
            {
                var otherResult = results.Where(r => r.MatchId == result.MatchId).ToList();

                if (otherResult.Count() < 2)
                {
                    result.Score = 1;
                    result.Win = true;
                    var matchCode = matches.Single(m => m.Id == result.MatchId).NextMachCode;
                    var matchId = matches.Single(m => m.MatchCode == matchCode).Id;
                    newResults.Add(new Result(result.PlayerId, matchId));
                }

                matches.First(m => m.Id == result.MatchId).EditAble = false;
            }
            
            return new TournamentParts()
            {
                Tournament = tournament,
                Matches = matches,
                Results = results.Concat(newResults),
                Players = players
            };
        }
        
        public IEnumerable<NodeDTO> GetFlatStructure(Tournament tournament)
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
                    NextMatch = match.NextMachCode != null
                        ? CreateNode(tournament, match.NextMachCode, true)
                        : null,
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

        public TournamentParts StartUpperTree(Tournament tournament)
        {

            var matches = CreateMatches(tournament.Players.Count(), tournament.Id);
            
            var results = new List<Result>();
            
            foreach (var player in tournament.Players)
            {
                results.Add(new Result()
                {
                    PlayerId = player.Id,
                    Score = 0,
                    Win = false,
                });
            }

            var firstRoundIds = matches.Where(m => m.Round == 1).Select(m => m.Id).ToList();

            foreach (var result in results)
            {
                var match = firstRoundIds.FirstOrDefault(m => !results.Where(r => m == r.MatchId).ToList().Any());

                if (match != Guid.Empty)
                {
                    result.MatchId = match;
                }
                else
                {
                    match = firstRoundIds.First(m => results.Where(r => m == r.MatchId).ToList().Count == 1);
                    result.MatchId = match;
                }
            }
            
            var newResults = new List<Result>();

            foreach (var result in results)
            {
                var otherResult = results.Where(r => r.MatchId == result.MatchId).ToList();

                if (otherResult.Count() < 2)
                {
                    result.Score = 1;
                    result.Win = true;
                    var matchCode = matches.Single(m => m.Id == result.MatchId).NextMachCode;
                    var matchId = matches.Single(m => m.MatchCode == matchCode).Id;
                    newResults.Add(new Result(result.PlayerId, matchId));
                }

                matches.First(m => m.Id == result.MatchId).EditAble = false;
            }
            
            return new TournamentParts()
            {
                Matches = matches,
                Results = results.Concat(newResults),
            };
        }

        public async Task UpdateSingleEliminationTree(MatchDTO matchDto)
        {
            var match = _tournamentRepository.GetMatch(matchDto.Id);
            
            var nextMatch = _treeRepository.GetMatchByCode(match.NextMachCode, match.TournamentId);
            
            if (nextMatch != null && (match.Result.Count() != 2 || matchDto.Player1Score == matchDto.Player2Score
                                           || nextMatch.Result != null &&
                                           nextMatch.Result.SingleOrDefault(r => r.Win) != null))
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

            if (nextMatch != null && (nextMatch.Result != null &&
                 nextMatch.Result.SingleOrDefault(r => r.PlayerId == p1.PlayerId || r.PlayerId == p2.PlayerId) != null))
            {
                await _tournamentRepository.DeleteResult(nextMatch.Result.Single(r => r.PlayerId == p1.PlayerId || r.PlayerId == p2.PlayerId));
            }

            if (nextMatch != null)
            {
                var result = new Result(winner.PlayerId, nextMatch.Id); 
                await _tournamentRepository.AddResult(result);
            }
        }

        public async Task CreateOpenTournament(RegisterTournamentModel tournamentModel, Guid userId)
        {
            if (tournamentModel.Type == TournamentsTypes.Siwss)
            {
                var tournament = new Tournament(tournamentModel.Name, tournamentModel.Rounds, userId, tournamentModel.Type,
                    tournamentModel.MaxNumberOfPlayers, tournamentModel.GameType, tournamentModel.Address, tournamentModel.City,
                    tournamentModel.StartDate);
                var players = new List<Player>();
                    foreach (var player in tournamentModel.Players)
                    {
                        players.Add(new Player(player, tournament));
                    }
                    await _tournamentRepository.AddOpenTournamentWithPlayers(tournament, players);
            }
            else
            {
                var tournament = new Tournament(tournamentModel.Name, userId, tournamentModel.Type,
                    tournamentModel.MaxNumberOfPlayers, tournamentModel.GameType, tournamentModel.Address, tournamentModel.City,
                    tournamentModel.StartDate);
                var players = new List<Player>();
                foreach (var player in tournamentModel.Players)
                {
                    players.Add(new Player(player, tournament));
                }
                await _tournamentRepository.AddOpenTournamentWithPlayers(tournament, players);
            }
        }

        private NodeDTO CreateNode(Tournament tournament, string matchCode, bool recursionFlag)
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
                    Player1Score = p1Score != null ? p1Score.Value : 0,
                    Player2 = p1 != p2 ? p2 : null,
                    Player2Score = p1 != p2 ? p2Score != null ? p2Score.Value : 0 : 0,
                    EditAble = match.EditAble,
                    NextMatch = CreateNode(tournament, match.NextMachCode, false)
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

        private List<Match> CreateMatches(int numberOfPlayers, Guid tournamentId)
        {
            var res = new List<Match>();

            if (numberOfPlayers % 2 == 1)
            {
                numberOfPlayers++;
            }

            var roundCounter = (int) Math.Ceiling(Math.Log2(numberOfPlayers));

            var matchCount = 1;

            for (int i = roundCounter; i > 0; i--)
            {
                var prevRoundList = res.FindAll(m => m.Round == i + 1).ToList();

                for (int j = 0; j < matchCount; j++)
                {
                    var matchCode = GenerateMatchCode(i, j + 1);
                    var match = new Match(matchCode, i, tournamentId);

                    if (prevRoundList.Count != 0)
                    {
                        var tmp = j;
                        if (j % 2 == 1)
                        {
                            tmp--;
                        }

                        match.NextMachCode = prevRoundList[tmp / 2].MatchCode;
                    }

                    prevRoundList.Add(match);
                    res.Add(match);
                }

                matchCount *= 2;
            }

            return res;
        }
        
        private string GenerateMatchCode(int round, int matchNumber)
        {
            string res = (Convert.ToChar(round + 64)).ToString();
            res += matchNumber.ToString();
            return res;
        }
    }
}