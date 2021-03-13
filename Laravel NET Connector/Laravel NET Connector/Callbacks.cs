using System;
using System.Collections.Generic;
using System.Text;

namespace LaravelNETConnector
{
    public static class Callbacks
    {
        public static Func<dynamic> UnauthorizedCallBack { get; set; }

        public static Func<bool> NotConnectedCallBack { get; set; }
    }
}
