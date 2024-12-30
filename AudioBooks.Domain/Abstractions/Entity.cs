using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.Abstractions;

public abstract class Entity
{
    [Key]
    public Guid Id { get; set; }
}
