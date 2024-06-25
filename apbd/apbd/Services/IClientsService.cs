using apbd.DTOs;

namespace apbd.Services;

public interface IClientsService
{
    public Task<RentalsDTO?> GetClientWithRentedCars(int id);
    public Task<int?> CarExistsAndReturnPrice(int id);
    public int? CalculateTotalPrice(DateTime dateFrom, DateTime dateTo, int? pricePerDay);
    public Task<int> AddClientAsync(string firstname, string lastname, string address);
    public Task<int> AddCarToClientAsync(PostDTO postDto, int? clientid, int? totalPrice);
    public Task<string> AddNewClientWithRental(PostDTO postDto);
}