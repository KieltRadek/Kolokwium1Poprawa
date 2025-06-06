using System.Collections.Generic;

namespace Kolokwium1Poprawa.Domain
{
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<Rental> Rentals { get; set; } = new();
    }
}