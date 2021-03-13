using System;
using System.Collections.Generic;
using System.Text;
using LaravelNETConnector.Models;

namespace LaravelNETConnector
{
    public static class Authentication
    {
        private static Authenticatable currentUser;

        public static Authenticatable CurrentUser
        {
            get => currentUser;
            internal set
            {
                currentUser = value;

                Events.OnCurrentUserChanged(value);
            }
        }
    }
}
