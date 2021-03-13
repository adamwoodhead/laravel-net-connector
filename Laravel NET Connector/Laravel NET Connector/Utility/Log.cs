using System;
using System.Collections.Generic;
using System.Text;

namespace LaravelNETConnector.Utility
{
    internal static class Log
    {
        public static void WriteLine(string message)
        {
            Host.Config.Out?.WriteLine(message);
        }
    }
}
