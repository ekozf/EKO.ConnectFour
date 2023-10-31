namespace EKO.ConnectFour.Api.Models;

public sealed class GameChatUserModel : ChatUserModel
{
    public Guid GameId { get; set; } = Guid.Empty;
}
