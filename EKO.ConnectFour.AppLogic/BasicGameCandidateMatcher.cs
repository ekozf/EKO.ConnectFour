using EKO.ConnectFour.AppLogic.Contracts;
using EKO.ConnectFour.Domain.GameDomain.Contracts;

namespace EKO.ConnectFour.AppLogic;

/// <inheritdoc cref="IGameCandidateMatcher"/>
public class BasicGameCandidateMatcher : IGameCandidateMatcher
{
    public IGameCandidate SelectOpponentToChallenge(IList<IGameCandidate> possibleOpponents)
    {
        if (possibleOpponents == null || possibleOpponents.Count == 0)
        {
            return null;
        }

        return possibleOpponents[0];
    }
}