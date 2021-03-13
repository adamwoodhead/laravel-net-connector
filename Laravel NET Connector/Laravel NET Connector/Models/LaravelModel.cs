using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LaravelNETConnector.Attributes;
using Newtonsoft.Json;
using RestSharp;

namespace LaravelNETConnector.Models.DataModels
{
    [JsonObject]
    public class LaravelModel<T>
    {
        [JsonProperty("id")]
        public int? ID { get; set; } = null;

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("errors")]
        public List<string> Errors { get; set; }

        #region Pagination

        public static async Task<Page<T>> PaginateAsync(int pageNumber = 1, int perPage = 50, CancellationToken cancellationToken = default)
        {
            return await Page<T>.GetAsync(pageNumber, perPage, cancellationToken);
        }

        public static async Task<Page<T>> PaginateSearchAsync(string haystack, string needle, int pageNumber = 1, int perPage = 50, CancellationToken cancellationToken = default)
        {
            return await Page<T>.SearchAsync(pageNumber, perPage, haystack, needle, cancellationToken);
        }

        #endregion

        #region GET

        // TODO Add *with* functionality, preferrably by passing class name

        public static async Task<List<T>> GetAsync(CancellationToken cancellationToken = default)
        {
            RouteAttribute routeAttribute = typeof(T).GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(RouteAttribute)) as RouteAttribute;

            string url = routeAttribute.GetIndexRoute();

            RestRequest request = new RestRequest(url, Method.GET, DataFormat.Json);

            IRestResponse<List<T>> restResponse = await Host.RequestAsync<List<T>>(request, cancellationToken);

            return restResponse.Data;
        }

        public static async Task<T> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            RouteAttribute routeAttribute = typeof(T).GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(RouteAttribute)) as RouteAttribute;

            string url = routeAttribute.GetSingularRoute(id);

            RestRequest request = new RestRequest(url, Method.GET, DataFormat.Json);

            IRestResponse<T> restResponse = await Host.RequestAsync<T>(request, cancellationToken);

            return restResponse.Data;
        }

        public static async Task<List<T>> SearchAsync(string haystack, string needle, CancellationToken cancellationToken = default)
        {
            RouteAttribute routeAttribute = typeof(T).GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(RouteAttribute)) as RouteAttribute;

            string url = routeAttribute.GetSearchRoute(haystack, needle);

            RestRequest request = new RestRequest(url, Method.GET, DataFormat.Json);

            IRestResponse<List<T>> restResponse = await Host.RequestAsync<List<T>>(request, cancellationToken);

            return restResponse.Data;
        }

        #endregion

        #region POST

        public virtual async Task<T> CreateAsync(CancellationToken cancellationToken = default)
        {
            RouteAttribute routeAttribute = this.GetType().GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(RouteAttribute)) as RouteAttribute;

            string url = routeAttribute.GetIndexRoute();

            RestRequest request = new RestRequest(url, Method.POST, DataFormat.Json);

            request.AddJsonBody(this);

            IRestResponse<T> restResponse = await Host.RequestAsync<T>(request, cancellationToken);

            return restResponse.Data;
        }
        #endregion

        #region UPDATE

        public virtual async Task<T> UpdateAsync(CancellationToken cancellationToken = default)
        {
            RouteAttribute routeAttribute = this.GetType().GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(RouteAttribute)) as RouteAttribute;

            string url = routeAttribute.GetSingularRoute(ID);

            RestRequest request = new RestRequest(url, Method.PUT, DataFormat.Json);

            request.AddJsonBody(this);

            IRestResponse<T> restResponse = await Host.RequestAsync<T>(request, cancellationToken);

            return restResponse.Data;
        }

        #endregion

        #region DELETE

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken = default)
        {
            RouteAttribute routeAttribute = this.GetType().GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(RouteAttribute)) as RouteAttribute;

            string url = routeAttribute.GetSingularRoute(ID);

            RestRequest request = new RestRequest(url, Method.DELETE, DataFormat.Json);

            IRestResponse<T> restResponse = await Host.RequestAsync<T>(request, cancellationToken);

            return restResponse.IsSuccessful;
        }

        #endregion
    }
}
