using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class TreeHub : Hub<ITreeHubClient>
    {
        public async Task SendHello()
        {
            await Clients.All.ReceiveHello("A");
        }
    }
}
