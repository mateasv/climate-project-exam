using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinBase.EventArguments
{
    public class ConnectionEventArgs
    {
        public Exception Exception { get; set; }
        public bool IsConnected { get; set; }
    }
}
