using EKO.ConnectFour.AppLogic.Contracts;
using EKO.ConnectFour.Domain;
using EKO.ConnectFour.Domain.GameDomain;
using EKO.ConnectFour.Domain.GameDomain.Contracts;

namespace EKO.ConnectFour.AppLogic;

public class LiveWaitingPool : ILiveWaitingPool
{
    private readonly IGameCandidateFactory _gameCandidateFactory;
    private readonly IGameCandidateRepository _gameCandidateRepository;
    private readonly IGameCandidateMatcher _gameCandidateMatcher;
    private readonly IGameService _gameService;

    public LiveWaitingPool(IGameCandidateFactory gameCandidateFactory, IGameCandidateRepository gameCandidateRepository, IGameCandidateMatcher gameCandidateMatcher, IGameService gameService)
    {
        _gameCandidateFactory = gameCandidateFactory;
        _gameCandidateRepository = gameCandidateRepository;
        _gameCandidateMatcher = gameCandidateMatcher;
        _gameService = gameService;
    }

    public Guid JoinLive(User user, GameSettings gameSettings)
    {
        var candidate = _gameCandidateFactory.CreateNewForUser(user, gameSettings);

        if (!gameSettings.AutoMatchCandidates)
            return Guid.Empty;

        _gameCandidateRepository.AddOrReplace(candidate);

        var challengers = _gameCandidateRepository.FindCandidatesThatCanBeChallengedBy(user.Id);

        var opponent = _gameCandidateMatcher.SelectOpponentToChallenge(challengers);

        if (opponent == null)
            return Guid.Empty;

        candidate.Challenge(opponent);

        opponent.AcceptChallenge(candidate);

        var game = _gameService.CreateGameForUsers(candidate.User, opponent.User, gameSettings);

        candidate.GameId = game.Id;
        opponent.GameId = game.Id;

        //var liveMatchFound = new LiveMatchFound

        return game.Id;
    }

    public void LeaveLive(Guid userId)
    {
        _gameCandidateRepository.RemoveCandidate(userId);
    }
}
