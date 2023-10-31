namespace EKO.ConnectFour.Api.Models;

public class PingResultModel
{
    public bool IsAlive { get; set; }
    public required string Greeting { get; set; }
    public DateTime ServerTime { get; set; }
}