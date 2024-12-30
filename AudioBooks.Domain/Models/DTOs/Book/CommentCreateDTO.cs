using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.DTOs.Book;

public class CommentCreateDTO
{
    [Range(1, 5, ErrorMessage = "CommentValue faqat 1 dan 5 gacha bo'lishi kerak.")]
    public int CommentValue { get; set; }
    //public string? UserName { get; set; }
    public string? Text { get; set; }
}
