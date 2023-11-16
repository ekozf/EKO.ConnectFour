using EKO.ConnectFour.Domain;
using EKO.ConnectFour.Domain.GameDomain;

namespace EKO.ConnectFour.AppLogic.Contracts;

public interface ILiveWaitingPool
{
    Guid JoinLive(User user, GameSettings gameSettings);

    void LeaveLive(Guid userId);
}
