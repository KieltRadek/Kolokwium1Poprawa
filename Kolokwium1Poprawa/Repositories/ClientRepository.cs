using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Kolokwium1Poprawa.Domain;
using Microsoft.Extensions.Configuration;

namespace Kolokwium1Poprawa.Repositories
{
  public class ClientRepository : IClientRepository
  {
    private readonly string _conn;

    public ClientRepository(IConfiguration config)
    {
      _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public async Task<Client?> GetClientWithRentalsAsync(int clientId)
    {
      await using var conn = new SqlConnection(_conn);
      await conn.OpenAsync();

      await using var cmdC = new SqlCommand(
        "SELECT ID, FirstName, LastName, Address FROM clients WHERE ID = @ID", conn);
      cmdC.Parameters.AddWithValue("@ID", clientId);

      await using var r = await cmdC.ExecuteReaderAsync();
      if (!await r.ReadAsync()) return null;

      var client = new Client
      {
        Id = r.GetInt32(0),
        FirstName = r.GetString(1),
        LastName = r.GetString(2),
        Address = r.GetString(3)
      };
      await r.CloseAsync();

      await using var cmdR = new SqlCommand(
        @"SELECT c.VIN,
                 col.Name AS Color,
                 m.Name AS Model,
                 r.DateFrom,
                 r.DateTo,
                 r.TotalPrice
          FROM car_rentals r
          JOIN cars c ON r.CarID = c.ID
          JOIN colors col ON c.ColorID = col.ID
          JOIN models m ON c.ModelID = m.ID
          WHERE r.ClientID = @CID", conn);
      cmdR.Parameters.AddWithValue("@CID", clientId);

      await using var r2 = await cmdR.ExecuteReaderAsync();
      while (await r2.ReadAsync())
      {
        client.Rentals.Add(new Rental
        {
          VIN = r2.GetString(0),
          Color = r2.GetString(1),
          Model = r2.GetString(2),
          DateFrom = r2.GetDateTime(3),
          DateTo = r2.GetDateTime(4),
          TotalPrice = r2.GetInt32(5)
        });
      }

      return client;
    }

    public async Task<int> CreateClientWithRentalAsync(
        string firstName,
        string lastName,
        string address,
        int carId,
        DateTime dateFrom,
        DateTime dateTo)
    {
        await using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var tran = conn.BeginTransaction();

        try
        {
            using (var cmdPrice = new SqlCommand(
                       "SELECT PricePerDay FROM cars WHERE ID = @CarID", conn, tran))
            {
                cmdPrice.Parameters.AddWithValue("@CarID", carId);
                var priceObj = await cmdPrice.ExecuteScalarAsync();
                if (priceObj == null)
                    throw new KeyNotFoundException($"Car {carId} not found");

                int ppd = Convert.ToInt32(priceObj);

                using var cmdInsC = new SqlCommand(
                    @"INSERT INTO clients (FirstName, LastName, Address)
                  OUTPUT INSERTED.ID
                  VALUES (@F, @L, @A)", conn, tran);
                cmdInsC.Parameters.AddWithValue("@F", firstName);
                cmdInsC.Parameters.AddWithValue("@L", lastName);
                cmdInsC.Parameters.AddWithValue("@A", address);
                var newId = (int)await cmdInsC.ExecuteScalarAsync();

                var days = (dateTo - dateFrom).Days;
                var total = days * ppd;

                using var cmdInsR = new SqlCommand(
                    @"INSERT INTO car_rentals
                  (ClientID, CarID, DateFrom, DateTo, TotalPrice)
                  VALUES (@CID, @CarID, @DF, @DT, @TP)", conn, tran);
                cmdInsR.Parameters.AddWithValue("@CID", newId);
                cmdInsR.Parameters.AddWithValue("@CarID", carId);
                cmdInsR.Parameters.AddWithValue("@DF", dateFrom);
                cmdInsR.Parameters.AddWithValue("@DT", dateTo);
                cmdInsR.Parameters.AddWithValue("@TP", total);
                await cmdInsR.ExecuteNonQueryAsync();

                await tran.CommitAsync();
                return newId;
            }
        }
        catch
        {
            await tran.RollbackAsync();
            throw;
        }
    }
  }
}
