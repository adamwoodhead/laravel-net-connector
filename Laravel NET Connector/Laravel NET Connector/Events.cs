using System;
using System.Collections.Generic;
using System.Text;
using LaravelNETConnector.Models;

namespace LaravelNETConnector
{
    public static class Events
    {
        public static event EventHandler CurrentUserChanged;

        internal static void OnCurrentUserChanged(Authenticatable value)
        {
            CurrentUserChanged?.Invoke(value, null);
        }
    }
}
