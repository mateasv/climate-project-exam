using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinBase.EventArguments
{
    /// <summary>
    /// Arguments returned from a SignalR connect or disconnect event
    /// </summary>
    public class ConnectionEventArgs
    {
        public Exception Exception { get; set; }
        public bool IsConnected { get; set; }
    }
}
