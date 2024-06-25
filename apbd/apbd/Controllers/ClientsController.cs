using apbd.DTOs;
using apbd.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController: ControllerBase
{
    private readonly IClientsService _clientsService;

    public ClientsController(IClientsService clientsService)
    {
        _clientsService = clientsService;
    }

    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetRentalsAsync(int clientId)
    {
        RentalsDTO? rentalsDto = await _clientsService.GetClientWithRentedCars(clientId);

        if (rentalsDto is null)
        {
            return BadRequest($"Error: Cannot find client with id {clientId}.");
        }

        return Ok(rentalsDto);
    }

    [HttpPost]
    public async Task<IActionResult> PostClientWithRentalAsync(PostDTO postDto)
    {
        string message = await _clientsService.AddNewClientWithRental(postDto);
        if (message.StartsWith("Error"))
        {
            return BadRequest(message);
        }

        return Ok(message);
    }
    
}