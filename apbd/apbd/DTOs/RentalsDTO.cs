namespace apbd.DTOs;

public class RentalsDTO
{
    public int id { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string address { get; set; }
    public List<CarDTO> rentals { get; set; } = new List<CarDTO>();
}