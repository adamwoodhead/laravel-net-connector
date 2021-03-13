using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using LaravelNETConnector.Attributes;
using LaravelNETConnector.Models.DataModels;

namespace ExampleProject
{
    [Route("/invoices")]
    [JsonObject]
    public class Invoice : LaravelModel<Invoice>
    {
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("customer_id")]
        public int? CustomerID { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("invoice_date")]
        public DateTime InvoiceDate { get; set; }

        [JsonProperty("discount_flat")]
        public double? DiscountFlat { get; set; }

        [JsonProperty("discount_percent")]
        public double? DiscountPercentage { get; set; }

        // relations

        //[JsonProperty("items")]
        //public List<InvoiceItem> Items { get; set; }

        // assistants

        //public static async Task<List<InvoiceItem>> GetItemsAsync(int id)
        //{
        //    return await Invoice.GetRelatedModelListAsync<InvoiceItem>("items", id);
        //}

        //public async Task<List<InvoiceItem>> GetItemsAsync()
        //{
        //    return await GetItemsAsync((int)this.ID);
        //}
    }
}
