namespace apbd.DTOs;

public class PostDTO
{
    public ClientDTO client { get; set; }
    public int carId { get; set; }
    public DateTime dateFrom { get; set; }
    public DateTime dateTo { get; set; }
}