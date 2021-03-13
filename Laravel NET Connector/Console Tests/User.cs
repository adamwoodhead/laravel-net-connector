using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using LaravelNETConnector.Interfaces;
using LaravelNETConnector.Models;

namespace ExampleProject
{
    [JsonObject]
    public class User : Authenticatable, ITimestamps, ISoftDelete
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public DateTime? DeletedAt { get; set; }

        DateTime? ISoftDelete.CreatedAt { get; set; }

        DateTime? ISoftDelete.UpdatedAt { get; set; }

        public User(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
