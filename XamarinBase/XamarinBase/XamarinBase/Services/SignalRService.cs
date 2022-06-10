using Microsoft.AspNetCore.SignalR.Client;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XamarinBase.EventArguments;

namespace XamarinBase.Services
{
    /// <summary>
    /// SignalR service used for calling methods on the hub and register on warning events from
    /// the server.
    /// </summary>
    public class SignalRService : ISignalRService
    {
        public event EventHandler<WarningEventArgs> OnReceiveWarning;
        public event EventHandler<ConnectionEventArgs> OnConnectSuccess;
        public event EventHandler<ConnectionEventArgs> OnConnectFailed;

        private HubConnection _hubConnection;

        private string _connectionUrl;

        public string ConnectionUrl
        {
            get { return _connectionUrl; }
            set { _connectionUrl = value; }
        }

        public bool IsConnected { get; set; }

        public SignalRService()
        {
            ConnectionUrl = "https://10.0.2.2:7189/TreeHub";
        }

        /// <summary>
        /// Builds the instance of the hub connection
        /// </summary>
        public void Build()
        {
            // Building the hub connection
            _hubConnection = new HubConnectionBuilder().WithUrl(ConnectionUrl, (opts) =>
            {
                // option to disable SSL verification
                opts.HttpMessageHandlerFactory = (message) =>
                {
                    if (message is HttpClientHandler clientHandler)
                        // bypass SSL certificate
                        clientHandler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, sslPolicyErrors) => { return true; };
                    return message;
                };
            }).Build();

            // Start listening for ReceiveWarning events
            _hubConnection.On<Measurement, bool>("ReceiveWarning", (measurement, isWarning) =>
            {
                // Call subscribers of the OnReceiveWarning signalR event
                OnReceiveWarning?.Invoke(this, new WarningEventArgs() { Measurement = measurement, IsWarning = isWarning });
            });
        }

        /// <summary>
        /// Attempts to start the connection to the SignalR hub
        /// </summary>
        /// <returns>true if success else false</returns>
        public async Task<bool> StartAsync()
        {
            await _hubConnection.StartAsync();

            IsConnected = true;

            return IsConnected;
        }

        /// <summary>
        /// Method used to register the mobile app in SignalR hub backend
        /// </summary>
        /// <returns></returns>
        public async Task RegisterApp()
        {
            await _hubConnection.SendAsync("RegisterApp");
        }
    }
}
