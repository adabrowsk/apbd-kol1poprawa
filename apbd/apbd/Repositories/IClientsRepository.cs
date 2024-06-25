using apbd.DTOs;

namespace apbd.Repositories;

public interface IClientsRepository
{
    public Task<RentalsDTO?> GetClientWithRentedCars(int id);
    public Task<int?> CarExistsAndReturnPrice(int id);
    public Task<int> AddClientAsync(string firstname, string lastname, string address);
    public Task<int> AddCarToClientAsync(PostDTO postDto, int? clientid, int? totalPrice);
}