using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.Abstractions;

public abstract class BaseParametrs : Entity
{
    [Required]
    public DateTime CreateDate { get; set; }
    [Required]
    public DateTime UpdateDate { get; set; }
    public DateTime? DeleteDate { get; set; }
    [Required]
    public bool IsDelete { get; set; }
}
