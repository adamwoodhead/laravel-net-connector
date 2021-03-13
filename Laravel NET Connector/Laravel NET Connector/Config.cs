using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static LaravelNETConnector.Enums;

namespace LaravelNETConnector
{
    public class Config
    {
        /// <summary>
        /// Base route for Laravel API
        /// Example:
        /// https://example.com/api
        /// </summary>
        public string BaseURL { get; set; }

        /// <summary>
        /// Automatically Attempt To Refresh Token or Throw NotAuthenticatedException Upon 401
        /// </summary>
        public bool AutoRefresh { get; set; } = false;

        public AuthenticationType AuthenticationMode { get; set; } = AuthenticationType.Passport;

        public bool ThrowAuthenticationExceptions { get; set; } = true;

        public TextWriter Out { get; set; }

        public bool LogRequests { get; set; } = true;

        public bool LogRequestsWithDuration { get; set; } = true;
    }
}
