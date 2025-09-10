using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Item
{
    public int Id { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    // Navigation property
    public virtual Product Product { get; set; } = null!;
}
