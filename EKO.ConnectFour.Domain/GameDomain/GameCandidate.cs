using EKO.ConnectFour.Domain.GameDomain.Contracts;

namespace EKO.ConnectFour.Domain.GameDomain;

/// <inheritdoc cref="IGameCandidate"/>
internal class GameCandidate : IGameCandidate
{
    public User User { get; private set; }
    public GameSettings GameSettings { get; }
    public Guid GameId { get; set; }
    public Guid ProposedOpponentUserId { get; private set; }

    internal GameCandidate(User user, GameSettings gameSettings)
    {
        User = user;
        GameSettings = gameSettings;
    }

    public bool CanChallenge(IGameCandidate targetCandidate)
    {
        if (targetCandidate.GameId != Guid.Empty) return false;
        if (GameId != Guid.Empty) return false;
        if (User.Id == targetCandidate.User.Id) return false;
        if (!GameSettings.AutoMatchCandidates || !targetCandidate.GameSettings.AutoMatchCandidates) return false;

        return targetCandidate.GameSettings.Equals(GameSettings);
    }

    public void Challenge(IGameCandidate targetCandidate)
    {
        if (!CanChallenge(targetCandidate)) throw new InvalidOperationException();

        ProposedOpponentUserId = targetCandidate.User.Id;
    }

    public void AcceptChallenge(IGameCandidate challenger)
    {
        if (GameId != Guid.Empty) throw new InvalidOperationException("Candidate is already in game");
        if (challenger.ProposedOpponentUserId != Guid.Empty && challenger.ProposedOpponentUserId != User.Id) throw new InvalidOperationException("Challenger already has other candidate.");

        if (!CanChallenge(challenger)) return;

        ProposedOpponentUserId = challenger.User.Id;
    }

    public void WithdrawChallenge()
    {
        throw new NotImplementedException();
    }
}