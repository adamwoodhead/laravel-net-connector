using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaravelNETConnector.Models.Auth
{
    [JsonObject]
    public class EmailPassword
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public EmailPassword(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
