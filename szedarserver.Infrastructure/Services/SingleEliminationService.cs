using System;
using System.Collections.Generic;
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

        public SingleEliminationService(ITournamentRepository tournamentRepository, ITreeRepository treeRepository)
        {
            _tournamentRepository = tournamentRepository;
            _treeRepository = treeRepository;
        }

        public async Task<Tournament> CreateSingleEliminationTournament(RegisterTournamentModel tournamentData,
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

            await _tournamentRepository.CreateTournamentAsync(tournament, players, matches, results.Concat(newResults));

            return tournament;
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
                    EditAble = match.EditAble,
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