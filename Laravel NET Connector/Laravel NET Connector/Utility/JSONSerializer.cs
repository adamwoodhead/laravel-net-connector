using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LaravelNETConnector.OLDMODELS
{
    internal class JSONSerializer : IRestSerializer
    {
        public string Serialize(object obj) => JsonConvert.SerializeObject(obj);

#pragma warning disable CS0618 // Required by IRestSerializer -- RestSharp
        public string Serialize(Parameter parameter) => JsonConvert.SerializeObject(parameter.Value);
#pragma warning restore CS0618

        // public T Deserialize<T>(IRestResponse response) => JsonConvert.DeserializeObject<T>(response.Content);

        public T Deserialize<T>(IRestResponse response)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (Exception)
            {
                try
                {
                    // Lets try wrapping it, we may have a singular whilst requesting a list.

                    string wrappedContent = $"[{response.Content}]";
                    return JsonConvert.DeserializeObject<T>(wrappedContent);
                }
                catch (Exception)
                {
                    return default;
                }
            }
        }

        public string[] SupportedContentTypes { get; } =
        {
            "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
        };

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;
    }
}
