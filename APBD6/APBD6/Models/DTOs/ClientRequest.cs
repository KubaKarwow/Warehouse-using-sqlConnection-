using System.ComponentModel.DataAnnotations;

namespace APBD6.Models.DTOs;

public class ClientRequest
{
    [Required]
    public int IdProduct { get; set; }
    [Required]
    public int IdWarehouse { get; set; }
    [Required]
    [Range(1,int.MaxValue)]
    public int Amount { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    public ClientRequest()
    {
    }
    public ClientRequest(int idProduct, int idWarehouse, int amount, DateTime createdAt)
    {
        IdProduct = idProduct;
        IdWarehouse = idWarehouse;
        Amount = amount;
        CreatedAt = createdAt;
    }

    
}