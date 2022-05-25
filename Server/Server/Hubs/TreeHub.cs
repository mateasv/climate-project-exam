using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class TreeHub : Hub<ITreeHubClient>
    {
        private static readonly Dictionary<string,string> Dataloggers = new();

        
        public async Task SendHello()
        {
            await Clients.All.ReceiveHello("A");
        }

        public async Task<bool> RegisterDatalogger(string id)
        {
            if(Dataloggers.TryAdd(Context.ConnectionId, id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RegisterApp()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,"Apps");

            return true;
        }

        public override Task OnConnectedAsync()
        {
            
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (Dataloggers.ContainsValue(Context.ConnectionId))
            {
                Dataloggers.Remove(Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
