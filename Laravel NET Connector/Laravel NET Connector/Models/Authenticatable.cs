using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LaravelNETConnector.Interfaces;
using LaravelNETConnector.Models.Auth;
using Newtonsoft.Json;
using RestSharp;

namespace LaravelNETConnector.Models
{
    [JsonObject]
    public class Authenticatable
    {
        [JsonIgnore]
        public IToken AuthenticationToken { get; set; }

        [JsonIgnore]
        public static string AuthenticateRoute => "/authenticate";

        [JsonIgnore]
        public static string RefreshRoute => "/refresh";

        [JsonIgnore]
        public static string LogoutRoute => "/logout";

        [JsonIgnore]
        public static string CurrentUserRoute => "/me";

        public async Task<bool> AuthenticateAsync(EmailPassword authPacket, CancellationToken cancellationToken = default)
        {
            Utility.Log.WriteLine("Authenticating...");

            try
            {
                string url = $"{Host.Config.BaseURL}" + AuthenticateRoute;

                RestRequest request = new RestRequest(url, Method.POST, DataFormat.Json);

                request.AddJsonBody(authPacket);

                IToken token = null;
                IRestResponse restResponse;


                switch (Host.Config.AuthenticationMode)
                {
                    case Enums.AuthenticationType.Passport:
                        IRestResponse<PassportToken> passportResponse = await Host.ObservedExecuteAsync<PassportToken>(request, cancellationToken);
                        restResponse = passportResponse;
                        token = passportResponse.Data;
                        break;
                    case Enums.AuthenticationType.JWT:
                        IRestResponse<JsonWebToken> jwtResponse = await Host.ObservedExecuteAsync<JsonWebToken>(request, cancellationToken);
                        restResponse = jwtResponse;
                        token = jwtResponse.Data;
                        break;
                    default:
                        throw new ApplicationException("Invalid Authentication Mode.");
                }

                if ((int)restResponse.StatusCode == 0)
                {
                    Callbacks.NotConnectedCallBack?.Invoke();
                    return false;
                }

                Authentication.CurrentUser = new Authenticatable();
                Authentication.CurrentUser.AuthenticationToken = token;

                Utility.Log.WriteLine("Authentication Successful");

                return true;
            }
            catch (Exception ex)
            {
                if (Host.Config.ThrowAuthenticationExceptions)
                {
                    Utility.Log.WriteLine("Authentication Successful");
                    Utility.Log.WriteLine(ex.ToString());
                    throw ex;
                }

                return false;
            }
        }

        public async Task<bool> AuthenticateAsync(UsernamePassword authPacket, CancellationToken cancellationToken = default)
        {
            Utility.Log.WriteLine("Authenticating...");

            try
            {
                string url = $"{Host.Config.BaseURL}" + AuthenticateRoute;

                RestRequest request = new RestRequest(url, Method.POST, DataFormat.Json);

                request.AddJsonBody(authPacket);

                IRestResponse<IToken> restResponse = await Host.ObservedExecuteAsync<IToken>(request, cancellationToken);

                if ((int)restResponse.StatusCode == 0)
                {
                    Callbacks.NotConnectedCallBack?.Invoke();
                    return false;
                }

                Authentication.CurrentUser = new Authenticatable();
                Authentication.CurrentUser.AuthenticationToken = restResponse.Data;

                Utility.Log.WriteLine("Authentication Successful");

                return true;
            }
            catch (Exception ex)
            {
                if (Host.Config.ThrowAuthenticationExceptions)
                {
                    Utility.Log.WriteLine("Authentication Successful");
                    Utility.Log.WriteLine(ex.ToString());
                    throw ex;
                }

                return false;
            }
        }

        public async Task<bool> AuthenticateAsync(dynamic authPacket, CancellationToken cancellationToken = default)
        {
            Utility.Log.WriteLine("Authenticating...");

            try
            {
                string url = $"{Host.Config.BaseURL}" + AuthenticateRoute;

                RestRequest request = new RestRequest(url, Method.POST, DataFormat.Json);

                request.AddJsonBody(authPacket);

                IRestResponse<IToken> restResponse = await Host.ObservedExecuteAsync<IToken>(request, cancellationToken);

                if ((int)restResponse.StatusCode == 0)
                {
                    Callbacks.NotConnectedCallBack?.Invoke();
                    return false;
                }

                Authentication.CurrentUser = new Authenticatable();
                Authentication.CurrentUser.AuthenticationToken = restResponse.Data;

                Utility.Log.WriteLine("Authentication Successful");

                return true;
            }
            catch (Exception ex)
            {
                if (Host.Config.ThrowAuthenticationExceptions)
                {
                    Utility.Log.WriteLine("Authentication Successful");
                    Utility.Log.WriteLine(ex.ToString());
                    throw ex;
                }

                return false;
            }
        }

        internal async Task<bool> RefreshAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(AuthenticationToken?.Token) || cancellationToken.IsCancellationRequested)
            {
                AuthenticationToken = null;
                return false;
            }

            try
            {
                string url = $"{Host.Config.BaseURL}" + RefreshRoute;

                RestRequest request = new RestRequest(url, Method.POST, DataFormat.Json);

                request.AddHeader("Authorization", $"bearer {AuthenticationToken?.Token}");

                IRestResponse<IToken> restResponse = await Host.ObservedExecuteAsync<IToken>(request, cancellationToken);

                if (restResponse.IsSuccessful)
                {
                    AuthenticationToken = restResponse.Data;

                    Utility.Log.WriteLine("Token Refreshed Successfully");
                    return true;
                }
                else
                {
                    Utility.Log.WriteLine("Token Refresh Unsuccessful");
                    return false;
                }
            }
            catch (Exception ex)
            {
                AuthenticationToken = null;

                if (Host.Config.ThrowAuthenticationExceptions)
                {
                    throw ex;
                }

                return false;
            }
        }

        internal async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(AuthenticationToken?.Token))
            {
                // Log.Verbose("Logging Out...");

                try
                {
                    string url = $"{Host.Config.BaseURL}" + LogoutRoute;

                    RestRequest request = new RestRequest(url, Method.POST, DataFormat.Json);

                    request.AddHeader("Authorization", $"bearer {AuthenticationToken.Token}");

                    IRestResponse restResponse = await Host.RestClient.ExecuteAsync(request, cancellationToken);

                    if (restResponse.IsSuccessful)
                    {
                        Utility.Log.WriteLine("Token Sucessfully Invalidated");
                    }

                    AuthenticationToken = null;
                }
                catch (Exception ex)
                {
                    AuthenticationToken = null;

                    if (Host.Config.ThrowAuthenticationExceptions)
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}
