using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Models.BookModels;

namespace AudioBooks.Application.Interfaces.Books;

public interface IBookService
{
    Task<Result<IEnumerable<BookResultDTO>>> GetAllBook();
    Task<Result<BookResultDTO>> GetBook(Guid id);
    Task<RequestResponseDto> CreateBookAsync(BookCreateDTO book);
    Task<RequestResponseDto> UpdateBook(BookUpdateDTO book);
    Task<RequestResponseDto<Book>> DeleteBook(BookDeleteDTO book);
}
