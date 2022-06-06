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

        public void Build()
        {
            _hubConnection = new HubConnectionBuilder().WithUrl(ConnectionUrl, (opts) =>
            {
                opts.HttpMessageHandlerFactory = (message) =>
                {
                    if (message is HttpClientHandler clientHandler)
                        // bypass SSL certificate
                        clientHandler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, sslPolicyErrors) => { return true; };
                    return message;
                };
            }).Build();

            _hubConnection.On<Measurement, bool>("ReceiveWarning", (measurement, isWarning) =>
            {
                OnReceiveWarning?.Invoke(this, new WarningEventArgs() { Measurement = measurement, IsWarning = isWarning });
            });
        }

        public async Task<bool> StartAsync()
        {


            await _hubConnection.StartAsync();

            IsConnected = true;

            return IsConnected;
        }

        public async Task RegisterApp()
        {
            await _hubConnection.SendAsync("RegisterApp");
        }
    }
}
