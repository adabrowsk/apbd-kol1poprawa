using apbd.DTOs;
using Microsoft.Data.SqlClient;

namespace apbd.Repositories;

public class ClientsRepository: IClientsRepository
{
    private readonly IConfiguration _configuration;

    public ClientsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<RentalsDTO?> GetClientWithRentedCars(int id)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:Default"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT clients.ID AS cid, FirstName, LastName, [Address], vin, colors.Name AS cname, models.Name AS mname, DateFrom, DateTo, TotalPrice from clients INNER JOIN car_rentals ON car_rentals.ClientID=clients.ID" +
                          " INNER JOIN cars ON cars.ID=car_rentals.CarID INNER JOIN colors ON colors.ID=cars.ColorID INNER JOIN models ON models.ID=cars.ModelID WHERE clients.ID = @id";
        cmd.Parameters.AddWithValue("@id", id);

        int clientid = 0;
        string firstname = "";
        string lastname = "";
        string address = "";
        List<CarDTO> cars = new List<CarDTO>();
        
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            clientid = (int)reader["cid"];
            firstname = (string)reader["FirstName"];
            lastname = (string)reader["LastName"];
            address = (string)reader["Address"];
            cars.Add(new CarDTO()
            {
                vin = (string)reader["vin"],
                color = (string)reader["cname"],
                model = (string)reader["mname"],
                dateFrom = (DateTime)reader["DateFrom"],
                dateTo = (DateTime)reader["DateTo"],
                totalPrice = (int)reader["TotalPrice"],
            });
        }

        if (clientid == 0)
        {
            return null;
        }

        return new RentalsDTO
        {
            id = clientid,
            firstName = firstname,
            lastName = lastname,
            address = address,
            rentals = cars
        };
    }

    public async Task<int?> CarExistsAndReturnPrice(int id)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:Default"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "select PricePerDay from cars WHERE ID = @id";
        cmd.Parameters.AddWithValue("@id", id);
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return (int)reader["PricePerDay"];
        }

        return null;
    }

    public async Task<int> AddClientAsync(string firstname, string lastname, string address)
    {
        await using var con = new SqlConnection(_configuration.GetConnectionString("Default"));
        await con.OpenAsync();
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "INSERT INTO clients(FirstName, LastName, Address) VALUES (@fname, @lname, @address); SELECT @@IDENTITY AS ID;";
        cmd.Parameters.AddWithValue("@fname", firstname);
        cmd.Parameters.AddWithValue("@lname", lastname);
        cmd.Parameters.AddWithValue("@address", address);
        
        var res = await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(res);
    }

    public async Task<int> AddCarToClientAsync(PostDTO postDto, int? clientid, int? totalPrice)
    {
        await using var con = new SqlConnection(_configuration.GetConnectionString("Default"));
        await con.OpenAsync();
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "INSERT INTO car_rentals(ClientID, CarID, DateFrom, DateTo, TotalPrice, Discount) VALUES (@clid, @carid, @dfrom, @dto, @tp, @d)";
        cmd.Parameters.AddWithValue("@clid", clientid);
        cmd.Parameters.AddWithValue("@carid", postDto.carId);
        cmd.Parameters.AddWithValue("@dfrom", postDto.dateFrom);
        cmd.Parameters.AddWithValue("@dto", postDto.dateTo);
        cmd.Parameters.AddWithValue("@tp", totalPrice);
        cmd.Parameters.AddWithValue("@d", 0);
        
        var res = await cmd.ExecuteNonQueryAsync();

        return res;
    }
}