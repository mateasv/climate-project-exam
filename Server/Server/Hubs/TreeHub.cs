using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class TreeHub : Hub<ITreeHubClient>
    {
        private static readonly Dictionary<string,int> Dataloggers = new();

        /// <summary>
        /// Dataloggers must call this method with their id first in order to further
        /// subsribe to the methods described in the ITreeHubClient.
        /// </summary>
        /// <param name="id">id of the datalogger</param>
        /// <returns></returns>
        public async Task RegisterDatalogger(int id)
        {
            // Run the if statement if the Dataloggers dictionary does not contain the connection id as key and datalogger id as value
            if (!Dataloggers.ContainsKey(Context.ConnectionId) && !Dataloggers.ContainsValue(id))
            {
                // Adds the datalogger to a group for itself
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Datalogger: {id}");

                // Add the dataloggers id and connection id into the dictionary
                Dataloggers.Add(Context.ConnectionId, id);
            }
        }

        /// <summary>
        /// Mobile apps must call this method in order to receive relevant information regarding
        /// low temperature or dry soil.
        /// </summary>
        /// <returns></returns>
        public async Task RegisterApp()
        {
            // Adds the mobile apps connection id into the AppClient group
            await Groups.AddToGroupAsync(Context.ConnectionId,"AppClient");
        }

        /// <summary>
        /// Remove the datalogger entry in the dataloggers dictionary, when the datalogger is disconnected
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // If the dataloggers connection id is not in the dataloggers dictionary
            if (!Dataloggers.ContainsKey(Context.ConnectionId))
            {
                // Remove
                Dataloggers.Remove(Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
