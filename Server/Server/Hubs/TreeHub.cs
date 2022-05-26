using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class TreeHub : Hub<ITreeHubClient>
    {
        private static readonly Dictionary<string,int> Dataloggers = new();
        public async Task SendHello()
        {
            await Clients.All.ReceiveHello("A");
        }

        public async Task RegisterDatalogger(int id)
        {
            if (!Dataloggers.ContainsKey(Context.ConnectionId) && !Dataloggers.ContainsValue(id))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Datalogger: {id}");
                Dataloggers.Add(Context.ConnectionId, id);
            }
        }

        public async Task RegisterApp()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,"AppClient");
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (!Dataloggers.ContainsKey(Context.ConnectionId))
            {
                Dataloggers.Remove(Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
