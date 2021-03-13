using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaravelNETConnector.Models.Auth
{
    [JsonObject]
    public class UsernamePassword
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
