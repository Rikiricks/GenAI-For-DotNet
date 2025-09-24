using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TextCompletion.Models
{
    public class CarDetails
    {
        public required string Condition { get; set; }  // e.g. "New" or "Used"
        public required string Make { get; set; }
        public required string Model { get; set; }
        public int Year { get; set; }
        public CarListingType ListingType { get; set; }
        public int Price { get; set; }
        public required string[] Features { get; set; }
        public required string TenWordSummary { get; set; }

        public required string CarType { get; set; }     // e.g. "SUV", "Sedan", "Truck", etc.
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CarListingType { Sale, Lease }
}
