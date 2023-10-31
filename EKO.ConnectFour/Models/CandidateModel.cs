using AutoMapper;
using EKO.ConnectFour.Domain.GameDomain;
using EKO.ConnectFour.Domain.GameDomain.Contracts;

namespace EKO.ConnectFour.Api.Models;

public class CandidateModel
{
    /// <summary>
    /// Information about the user in the waiting pool.
    /// </summary>
    public UserModel User { get; set; }

    /// <summary>
    /// The game settings the user has chosen.
    /// </summary>
    public GameSettings GameSettings { get; set; }

    /// <summary>
    /// The id of the game that was created for the candidate.
    /// When this is not an empty guid, you know a game has been created for this candidate.
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// Id of a candidate that was challenged by this candidate.
    /// </summary>
    public Guid ProposedOpponentUserId { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IGameCandidate, CandidateModel>();
        }
    }
}