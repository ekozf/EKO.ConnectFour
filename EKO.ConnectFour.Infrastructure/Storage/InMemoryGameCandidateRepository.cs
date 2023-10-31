using EKO.ConnectFour.AppLogic.Contracts;
using EKO.ConnectFour.Common;
using EKO.ConnectFour.Domain.GameDomain.Contracts;

namespace EKO.ConnectFour.Infrastructure.Storage;

/// <inheritdoc cref="IGameCandidateRepository"/>
public class InMemoryGameCandidateRepository : IGameCandidateRepository
{
    private readonly ExpiringDictionary<Guid, IGameCandidate> _candidateDictionary;

    public InMemoryGameCandidateRepository()
    {
        _candidateDictionary = new ExpiringDictionary<Guid, IGameCandidate>(TimeSpan.FromMinutes(10));
    }

    public void AddOrReplace(IGameCandidate candidate)
    {
        _candidateDictionary.AddOrReplace(candidate.User.Id, candidate);
    }

    public void RemoveCandidate(Guid userId)
    {
        _candidateDictionary.TryRemove(userId, out IGameCandidate _);
    }

    public IGameCandidate GetCandidate(Guid userId)
    {
        if (_candidateDictionary.TryGetValue(userId, out IGameCandidate candidate))
        {
            return candidate;
        }
        throw new DataNotFoundException();
    }

    public IList<IGameCandidate> FindCandidatesThatCanBeChallengedBy(Guid userId)
    {
        if (_candidateDictionary.Values.Count == 0)
        {
            return new List<IGameCandidate>();
        }

        //TODO: retrieve the candidate with userId as key in the _candidateDictionary (use the TryGetValue method)
        var candidate = GetCandidate(userId);

        //TODO: loop over all candidates (user the Values property of _candidateDictionary)
        //and check if those candidates can be challenged by the candidate you retrieved in the first step (use the CanChallenge method of IGameCandidate).
        //Put the candidates that can be challenged in a list and return that list.
        var challengers = new List<IGameCandidate>();

        foreach (var challenger in _candidateDictionary.Values)
        {
            if (candidate.CanChallenge(challenger))
            {
                challengers.Add(challenger);
            }
        }

        return challengers;
    }

    public IList<IGameCandidate> FindChallengesFor(Guid challengedUserId)
    {
        return _candidateDictionary.Values.Where(t => t.ProposedOpponentUserId == challengedUserId).ToList();
    }
}