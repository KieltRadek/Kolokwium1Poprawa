using System;
using System.Threading.Tasks;
using Kolokwium1Poprawa.Domain;

namespace Kolokwium1Poprawa.Repositories
{
    public interface IClientRepository
    {
        Task<Client?> GetClientWithRentalsAsync(int clientId);
        Task<int> CreateClientWithRentalAsync(
            string firstName,
            string lastName,
            string address,
            int carId,
            DateTime dateFrom,
            DateTime dateTo);
    }
}