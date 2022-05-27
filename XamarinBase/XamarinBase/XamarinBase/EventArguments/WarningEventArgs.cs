using Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinBase.EventArguments
{
    public class WarningEventArgs
    {
        public Measurement Measurement { get; set; }
        public bool IsWarning { get; set; }
    }
}
