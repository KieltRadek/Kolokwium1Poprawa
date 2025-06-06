using System.Collections.Generic;

namespace Kolokwium1Poprawa.DTOs
{
    public class ClientDetailsDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<RentalDto> Rentals { get; set; } = new();
    }
}