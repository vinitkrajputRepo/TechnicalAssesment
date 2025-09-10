using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    [Required]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    
    [StringLength(100)]
    public string? ModifiedBy { get; set; }
    
    public DateTime? ModifiedOn { get; set; }
    
    // Navigation property for related items
    public virtual ICollection<Domain.Entities.Item> Items { get; set; } = new List<Domain.Entities.Item>();
}
