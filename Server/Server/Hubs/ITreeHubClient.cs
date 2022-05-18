namespace Server.Hubs
{
    public interface ITreeHubClient
    {
        Task ReceiveHello(string msg);
    }
}
