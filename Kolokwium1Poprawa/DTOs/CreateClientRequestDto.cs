using System;
using System.ComponentModel.DataAnnotations;

namespace Kolokwium1Poprawa.DTOs
{
    public class NewClientDto
    {
        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
    }

    public class CreateClientRequestDto
    {
        [Required] public NewClientDto Client { get; set; } = null!;
        [Required] public int CarId { get; set; }
        [Required] public DateTime DateFrom { get; set; }
        [Required] public DateTime DateTo { get; set; }
    }
}