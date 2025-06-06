using System;

namespace Kolokwium1Poprawa.Domain
{
    public class Rental
    {
        public string VIN { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int TotalPrice { get; set; }
    }
}