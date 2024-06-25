namespace apbd.DTOs;

public class CarDTO
{
    public string vin { get; set; }
    public string color { get; set; }
    public string model { get; set; }
    public DateTime dateTo { get; set; }
    public DateTime dateFrom { get; set; }
    public int totalPrice { get; set; }
}