using System;
using System.Linq;
using System.Threading.Tasks;
using Kolokwium1Poprawa.Domain;
using Kolokwium1Poprawa.DTOs;
using Kolokwium1Poprawa.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Poprawa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _repo;

        public ClientsController(IClientRepository repo) => _repo = repo;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var client = await _repo.GetClientWithRentalsAsync(id);
            if (client == null)
                return NotFound();

            var dto = new ClientDetailsDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Address = client.Address,
                Rentals = client.Rentals
                    .Select(r => new RentalDto
                    {
                        VIN = r.VIN,
                        Color = r.Color,
                        Model = r.Model,
                        DateFrom = r.DateFrom,
                        DateTo = r.DateTo,
                        TotalPrice = r.TotalPrice
                    })
                    .ToList()
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientRequestDto req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (req.DateTo <= req.DateFrom)
                return BadRequest("DateTo must be after DateFrom.");

            try
            {
                var newId = await _repo.CreateClientWithRentalAsync(
                    req.Client.FirstName,
                    req.Client.LastName,
                    req.Client.Address,
                    req.CarId,
                    req.DateFrom,
                    req.DateTo);

                return CreatedAtAction(
                    nameof(Get),
                    new { id = newId },
                    null);
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
        }
    }
}