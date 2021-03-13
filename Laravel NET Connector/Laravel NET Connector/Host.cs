using LaravelNETConnector.OLDMODELS;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace LaravelNETConnector
{
    public static class Host
    {
        internal static RestClient RestClient { get; set; }

        public static Config Config { internal get; set; }

        private static bool IsInitialized { get; set; } = false;

        internal static bool IsRefreshing { get; set; }

        public static int SuccessfulRequests { get; internal set; } = 0;

        public static int FailedRequests { get; internal set; } = 0;

        public static void Initialize(Config config)
        {
            if (!IsInitialized)
            {
                Config = config;

                System.Net.WebRequest.DefaultWebProxy = null;
                ServicePointManager.UseNagleAlgorithm = false;

                RestClient = new RestClient(Config.BaseURL)
                {
                    Proxy = null,
                    ThrowOnAnyError = true
                };
                RestClient.RemoteCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                RestClient.UseSerializer(() => new JSONSerializer());
            }

            IsInitialized = true;
            Utility.Log.WriteLine("Initialized");
        }

        public static async Task<bool> AttemptForcedAuthentication()
        {
            dynamic authenticatedPacket = Callbacks.UnauthorizedCallBack?.Invoke();

            if (authenticatedPacket != null && !string.IsNullOrEmpty(authenticatedPacket.AccessToken))
            {
                if (Authentication.CurrentUser == null)
                {
                    Authentication.CurrentUser = await authenticatedPacket.GetAuthenticatedUser();
                }

                Authentication.CurrentUser.AuthenticationToken.Token = authenticatedPacket;
                Authentication.CurrentUser.AuthenticationToken.BeginAutoRefreshAsync();

                return true;
            }

            return false;
        }

        public static async Task AttemptLogout()
        {
            try
            {
                if (Authentication.CurrentUser?.AuthenticationToken != null)
                {
                    await Authentication.CurrentUser.LogoutAsync();

                    Authentication.CurrentUser = null;
                }
            }
            catch (Exception)
            {
                Authentication.CurrentUser = null;
            }
        }

        internal static async Task<IRestResponse<T>> ObservedExecuteAsync<T>(IRestRequest restRequest, CancellationToken cancellationToken = default)
        {
            if (Host.Config.LogRequests)
            {
                if (Host.Config.LogRequestsWithDuration)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    IRestResponse<T> restResponse = await RestClient.ExecuteAsync<T>(restRequest, cancellationToken);
                    stopwatch.Stop();
                    Utility.Log.WriteLine($"[{restRequest.Method}] {stopwatch.ElapsedMilliseconds:0000}ms {Config.BaseURL}{restRequest.Resource}");
                    return restResponse;
                }
                else
                {
                    IRestResponse<T> restResponse = await RestClient.ExecuteAsync<T>(restRequest, cancellationToken);
                    Utility.Log.WriteLine($"[{restRequest.Method}] {Config.BaseURL}{restRequest.Resource}");
                    return restResponse;
                }
            }
            else
            {
                return await RestClient.ExecuteAsync<T>(restRequest, cancellationToken);
            }
        }

        public static async Task<IRestResponse<T>> RequestAsync<T>(RestRequest restRequest, CancellationToken cancellationToken = default)
        {
            if (Host.IsRefreshing)
            {
                Utility.Log.WriteLine("Currently refreshing token! Waiting...");

                while (Host.IsRefreshing)
                {
                    await Task.Delay(10);
                }
            }

            if (Authentication.CurrentUser?.AuthenticationToken != null)
            {
                restRequest.AddHeader("Authorization", $"bearer {Authentication.CurrentUser.AuthenticationToken.Token}");
            }

            IRestResponse<T> restResponse = await ObservedExecuteAsync<T>(restRequest, cancellationToken);

            if (!restResponse.IsSuccessful)
            {
                FailedRequests++;
                if ((int)restResponse.StatusCode == 0)
                {
                    if (Callbacks.NotConnectedCallBack?.Invoke() ?? false)
                    {
                        RestRequest replicatedRequest = new RestRequest($"{Config.BaseURL}{restRequest.Resource}", restRequest.Method, restRequest.RequestFormat);
                        return await RequestAsync<T>(replicatedRequest, cancellationToken);
                    }
                }
                else if (restResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    RestRequest replicatedRequest = new RestRequest($"{Config.BaseURL}{restRequest.Resource}", restRequest.Method, restRequest.RequestFormat);

                    if (IsRefreshing)
                    {
                        Utility.Log.WriteLine("We sent this request whilst refreshing our token! Waiting...");

                        while (IsRefreshing)
                        {
                            await Task.Delay(10);
                        }

                        Utility.Log.WriteLine("Finished Refresh Task, lets try again!");

                        return await RequestAsync<T>(replicatedRequest, cancellationToken);
                    }

                    Authentication.CurrentUser = null;

                    Utility.Log.WriteLine($"{restResponse.StatusCode} : {restResponse.Content}");

                    if (await AttemptForcedAuthentication() == false)
                    {
                        return null;
                    }

                    return await RequestAsync<T>(replicatedRequest, cancellationToken);
                }
                else if (restResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }
                else
                {
                    Utility.Log.WriteLine($"{restResponse.StatusCode} : {restResponse.Content}");
                    throw new Exception($"{restResponse.StatusCode} : {restResponse.Content}");
                }
            }

            SuccessfulRequests++;
            return restResponse;
        }
    }
}
