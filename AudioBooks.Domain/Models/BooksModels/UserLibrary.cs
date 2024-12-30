using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.Models.BookModels;

public class UserLibrary
{
    [Required(ErrorMessage = "Id is required.")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "UserId is required.")]
    public Guid User_Id { get; set; }

    [Required(ErrorMessage = "BookId is required.")]
    public Guid Book_Id { get; set; }

    [Required(ErrorMessage = "AddedDate is required.")]
    public DateTime AddedDate { get; set; }
}
