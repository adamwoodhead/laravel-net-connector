using System;
using System.Collections.Generic;
using System.Text;

namespace LaravelNETConnector.Attributes
{
    public class RouteAttribute : Attribute
    {
        private string IndexRoute { get; set; }

        public string GetIndexRoute()
        {
            return $"{IndexRoute}";
        }

        public string GetPaginatedRoute(int pageNumber, int perPage)
        {
            return $"{IndexRoute}?page={pageNumber}&per_page={perPage}";
        }

        public string GetSingularRoute(int? id)
        {
            return $"{IndexRoute}/{id}";
        }

        public string GetSearchRoute(string haystackField, string needleValue)
        {
            return $"{IndexRoute}?search={needleValue}&in={haystackField}";
        }

        public string GetPaginatedSearchRoute(int pageNumber, int perPage, string haystack, string needle)
        {
            return $"{IndexRoute}?search={needle}&in={haystack}&page={pageNumber}&per_page={perPage}";
        }

        public RouteAttribute(string baseRoute)
        {
            IndexRoute = baseRoute;
        }
    }
}
