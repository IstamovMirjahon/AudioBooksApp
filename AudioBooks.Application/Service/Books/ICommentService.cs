using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs.Book;

namespace AudioBooks.Application.Interfaces.Books;

public interface ICommentService
{
    Task<Result<CommentResultDTO>> AddCommentAsync(Guid bookId, CommentCreateDTO commentDto, string username);
    Task<Result<IEnumerable<CommentResultDTO>>> GetCommentsByBookIdAsync(Guid bookId);
    Task<Result<double>> CalculateAverageCommentValueAsync(Guid bookId);
}
