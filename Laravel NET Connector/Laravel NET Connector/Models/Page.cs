using LaravelNETConnector.Attributes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LaravelNETConnector.Models
{
    [JsonObject]
    public class Page<T>
    {
        // These properties are filled by the first request

        [JsonProperty("path")] //"path": "http://localhost:8000/api/stock-transactions",
        public string NonPaginatedURL { get; set; }

        [JsonProperty("current_page")] //"current_page": 3,
        public int? CurrentPage { get; set; }

        // The following must be gained via each lazy load

        [JsonProperty("total")] //"total": 2750,
        public int? TotalItems { get; set; }

        [JsonProperty("per_page")] //"per_page": 1000,
        public int? ItemsPerPage { get; set; }

        [JsonProperty("from")] //"from": 2001,
        public int? ItemFrom { get; set; }

        [JsonProperty("to")] //"to": 2750,
        public int? ItemTo { get; set; }

        [JsonProperty("first_page_url")] //"first_page_url": "http://localhost:8000/api/stock-transactions?page=1",
        public string FirstPageURL { get; set; }


        [JsonProperty("last_page")] //"last_page": 3,
        public int? TotalPageCount { get; set; }

        [JsonProperty("last_page_url")] //"last_page_url": "http://localhost:8000/api/stock-transactions?page=3",
        public string LastPageURL { get; set; }

        [JsonProperty("prev_page_url")] //"prev_page_url": "http://localhost:8000/api/stock-transactions?page=2",
        public string PreviousPageURL { get; set; }

        [JsonProperty("next_page_url")] //"next_page_url": null,
        public string NextPageURL { get; set; }

        [JsonProperty("data")] //"data": [],
        public List<T> Data { get; set; }

        internal Page(string baseUrl, int currentPage)
        {
            NonPaginatedURL = baseUrl;
            CurrentPage = currentPage;
        }

        [JsonConstructor]
        public Page() { }

        public static async Task<Page<T>> GetAsync(int pageNumber, int perPage, CancellationToken cancellationToken = default)
        {
            RouteAttribute routeAttribute = typeof(T).GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(RouteAttribute)) as RouteAttribute;

            string url = routeAttribute.GetPaginatedRoute(pageNumber, perPage);

            RestRequest request = new RestRequest(url, Method.GET, DataFormat.Json);

            IRestResponse<Page<T>> restResponse = await Host.RequestAsync<Page<T>>(request, cancellationToken);

            return restResponse.Data;
        }

        public static async Task<Page<T>> SearchAsync(int pageNumber, int perPage, string haystack, string needle, CancellationToken cancellationToken = default)
        {
            RouteAttribute routeAttribute = typeof(T).GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(RouteAttribute)) as RouteAttribute;

            string url = routeAttribute.GetPaginatedSearchRoute(pageNumber, perPage, haystack, needle);

            RestRequest request = new RestRequest(url, Method.GET, DataFormat.Json);

            IRestResponse<Page<T>> restResponse = await Host.RequestAsync<Page<T>>(request, cancellationToken);

            return restResponse.Data;
        }
    }
}