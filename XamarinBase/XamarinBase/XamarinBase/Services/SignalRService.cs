using Microsoft.AspNetCore.SignalR.Client;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XamarinBase.EventArguments;

namespace XamarinBase.Services
{
    public class SignalRService
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
            ConnectionUrl = "https://localhost:7189/TreeHub";
        }

        public void Build()
        {
            _hubConnection = new HubConnectionBuilder().WithUrl(ConnectionUrl).Build();

            _hubConnection.On<Measurement, bool>("ReceiveWarning", (measurement, isWarning) =>
            {
                OnReceiveWarning?.Invoke(this, new WarningEventArgs() { Measurement = measurement, IsWarning = isWarning });
            });

        }

        public async Task StartAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
                IsConnected = true;
                OnConnectSuccess?.Invoke(this, new ConnectionEventArgs { IsConnected = true });
            }
            catch (Exception ex)
            {
                OnConnectFailed?.Invoke(this, new ConnectionEventArgs { IsConnected = false, Exception = ex });
            }
        }
    }
}
