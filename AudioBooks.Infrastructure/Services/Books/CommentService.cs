using AudioBooks.Application.Interfaces.Books;
using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Models.BookModels;
using Microsoft.EntityFrameworkCore;

namespace AudioBooks.Infrastructure.Services.Books
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CommentResultDTO>> AddCommentAsync(Guid bookId, CommentCreateDTO commentDto, string usernmae)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return Result<CommentResultDTO>.Failure(new Error("BookNotFound", "Book not found"));
            }
            

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                UserName = usernmae,
                Text = commentDto.Text,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            var commentResultDto = new CommentResultDTO
            {
                Id = comment.Id,
                UserName = comment.UserName,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt
            };

            return Result<CommentResultDTO>.Success(commentResultDto);
        }

        public async Task<Result<IEnumerable<CommentResultDTO>>> GetCommentsByBookIdAsync(Guid bookId)
        {
            var comments = await _context.Comments
                .Where(c => c.BookId == bookId)
                .Select(c => new CommentResultDTO
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Result<IEnumerable<CommentResultDTO>>.Success(comments.AsEnumerable());
        }
    }
}
