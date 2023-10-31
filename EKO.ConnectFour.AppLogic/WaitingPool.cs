using EKO.ConnectFour.AppLogic.Contracts;
using EKO.ConnectFour.Domain;
using EKO.ConnectFour.Domain.GameDomain;
using EKO.ConnectFour.Domain.GameDomain.Contracts;

namespace EKO.ConnectFour.AppLogic;

/// <inheritdoc cref="IWaitingPool"/>
public class WaitingPool : IWaitingPool
{
    private readonly IGameCandidateFactory gameCandidateFactory;
    private readonly IGameCandidateRepository gameCandidateRepository;
    private readonly IGameCandidateMatcher gameCandidateMatcher;
    private readonly IGameService gameService;

    public WaitingPool(
        IGameCandidateFactory gameCandidateFactory,
        IGameCandidateRepository gameCandidateRepository, 
        IGameCandidateMatcher gameCandidateMatcher, 
        IGameService gameService)
    {
        this.gameCandidateFactory = gameCandidateFactory;
        this.gameCandidateRepository = gameCandidateRepository;
        this.gameCandidateMatcher = gameCandidateMatcher;
        this.gameService = gameService;
    }

    public void Join(User user, GameSettings gameSettings)
    {
        var candidate = gameCandidateFactory.CreateNewForUser(user, gameSettings);

        if (!gameSettings.AutoMatchCandidates) return;

        gameCandidateRepository.AddOrReplace(candidate);

        var challengers = gameCandidateRepository.FindCandidatesThatCanBeChallengedBy(user.Id);

        var opponent = gameCandidateMatcher.SelectOpponentToChallenge(challengers);

        if (opponent == null) return;

        candidate.Challenge(opponent);

        opponent.AcceptChallenge(candidate);

        var game = gameService.CreateGameForUsers(candidate.User, opponent.User, gameSettings);

        candidate.GameId = game.Id;
        opponent.GameId = game.Id;
    }

    public void Leave(Guid userId)
    {
        gameCandidateRepository.RemoveCandidate(userId);
    }

    public IGameCandidate GetCandidate(Guid userId)
    {
        return gameCandidateRepository.GetCandidate(userId);
    }

    public void Challenge(Guid challengerUserId, Guid targetUserId)
    {
        throw new NotImplementedException();
    }

    public IList<IGameCandidate> FindCandidatesThatCanBeChallengedBy(Guid userId)
    {
        var possebleCandidates = new List<IGameCandidate>();
        var candidates = gameCandidateRepository.FindChallengesFor(userId);
        foreach (var candidate in candidates)
        {
            if (candidate.GameSettings.Equals(gameCandidateRepository.GetCandidate(userId).GameSettings));
            {
                // The candidate is waiting for an opponent and has the same game settings as the user.
                possebleCandidates.Add(candidate);
            }
        }

        return possebleCandidates;
    }

    public void WithdrawChallenge(Guid userId)
    {
        throw new NotImplementedException();
    }

    public IList<IGameCandidate> FindChallengesFor(Guid challengedUserId)
    {
        throw new NotImplementedException();
    }
}