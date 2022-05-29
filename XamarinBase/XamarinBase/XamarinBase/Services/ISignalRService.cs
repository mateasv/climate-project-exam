using System;
using System.Threading.Tasks;
using XamarinBase.EventArguments;

namespace XamarinBase.Services
{
    public interface ISignalRService
    {
        string ConnectionUrl { get; set; }
        bool IsConnected { get; set; }

        event EventHandler<ConnectionEventArgs> OnConnectFailed;
        event EventHandler<ConnectionEventArgs> OnConnectSuccess;
        event EventHandler<WarningEventArgs> OnReceiveWarning;

        void Build();
        Task StartAsync();
    }
}