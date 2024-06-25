using System.Transactions;
using apbd.DTOs;
using apbd.Repositories;

namespace apbd.Services;

public class ClientsService: IClientsService
{
    private readonly IClientsRepository _clientsRepository;

    public ClientsService(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public async Task<RentalsDTO?> GetClientWithRentedCars(int id)
    {
        return await _clientsRepository.GetClientWithRentedCars(id);
    }

    public async Task<int?> CarExistsAndReturnPrice(int id)
    {
        return await _clientsRepository.CarExistsAndReturnPrice(id);
    }

    public int? CalculateTotalPrice(DateTime dateFrom, DateTime dateTo, int? pricePerDay)
    {
        if (dateFrom >= dateTo)
        {
            return -1;
        }
        TimeSpan diff = dateTo.Subtract(dateFrom);
        int days = (int)diff.TotalDays;

        return days * pricePerDay;
    }

    public async Task<int> AddClientAsync(string firstname, string lastname, string address)
    {
        return await _clientsRepository.AddClientAsync(firstname, lastname, address);
    }

    public async Task<int> AddCarToClientAsync(PostDTO postDto, int? clientid, int? totalPrice)
    {
        return await _clientsRepository.AddCarToClientAsync(postDto, clientid, totalPrice);
    }

    public async Task<string> AddNewClientWithRental(PostDTO postDto)
    {
        int? pricepd = await CarExistsAndReturnPrice(postDto.carId);

        if (pricepd is null)
        {
            return $"Error: Cannot find car with id {postDto.carId}.";
        }

        int? totalPrice = CalculateTotalPrice(postDto.dateFrom, postDto.dateTo, pricepd);
        if (totalPrice == -1)
        {
            return "Error: DateFrom cannot be higher or equal to DateTo.";
        }
        
        try
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int idClient = await AddClientAsync(postDto.client.firstName, postDto.client.lastName, postDto.client.address);

                await AddCarToClientAsync(postDto, idClient, totalPrice);
                
                scope.Complete();
                return "Successfully added new client and client's first rental.";
            }
        }
        catch (Exception e)
        {
            return "Error: Transaction rollbacked." + e;
        }
    }
}