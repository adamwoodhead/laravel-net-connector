using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LaravelNETConnector.Interfaces;
using Newtonsoft.Json;
using RestSharp;

namespace LaravelNETConnector.Models
{
    [JsonObject]
    public class JsonWebToken : IToken
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("token_type")]
        public string TypeType { get; set; }

        [JsonProperty("expires_at")]
        public int ExpiresAt { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonIgnore]
        public CancellationTokenSource CancellationTokenSource { get; private set; } = new CancellationTokenSource();

        public JsonWebToken() { }

        public void BeginAutoRefreshAsync()
        {
            Task.Run(async () => {
                Utility.Log.WriteLine("Token Refresh Task Started");

                while (Authentication.CurrentUser?.AuthenticationToken.Token?.Equals(this) ?? false)
                {
                    Utility.Log.WriteLine($"Now: {DateTime.UtcNow}");
                    // Utility.Log.WriteLine($"Expires At: {Helper.UnixTimeStampToDateTime(ExpiresAt)}");
                    // Utility.Log.WriteLine($"Refresh At: {Helper.UnixTimeStampToDateTime(ExpiresAt - 30)}");
                    Utility.Log.WriteLine($"Expires In: {ExpiresIn}s");
                    Utility.Log.WriteLine($"Refresh In: {ExpiresIn - 30}s (30 seconds prior)");

                    await Task.Delay((ExpiresIn - 30) * 1000, CancellationTokenSource?.Token ?? default);
                    CancellationTokenSource?.Token.ThrowIfCancellationRequested();

                    Utility.Log.WriteLine("Attempting Token Refresh");
                    try
                    {
                        Host.IsRefreshing = true;
                        await Authentication.CurrentUser?.RefreshAsync(CancellationTokenSource?.Token ?? default);
                        Host.IsRefreshing = false;
                    }
                    catch (Exception)
                    {
                        Host.IsRefreshing = false;
                    }
                }
            });
        }
    }
}
