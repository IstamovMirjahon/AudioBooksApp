using AudioBooks.Application.Data;
using AudioBooks.Application.Service.UserServices;
using AudioBooks.Domain.Abstractions;
using AudioBooks.Infrastructure.Repositories.UserRepositories;
using AudioBooks.Infrastructure.Repositories;
using AudioBooks.Infrastructure.Services.UserServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AudioBooks.Infrastructure.Data;
using AudioBooks.Infrastructure.Repositories.BooksRepository;
using AudioBooks.Infrastructure.Repositories.CategorysBooksRepository;
using AudioBooks.Infrastructure.Repositories.MyBooks;
using AudioBooks.Application.Interfaces.Auth;
using AudioBooks.Infrastructure.Services.Auth;
using AudioBooks.Application.Interfaces.Books;
using AudioBooks.Infrastructure.Services.Books.MyBooks;
using AudioBooks.Infrastructure.Services.Books;
using AudioBooks.Application.Interfaces.Email;
using AudioBooks.Domain.Interfaces.Token;
using AudioBooks.Infrastructure.Services.Email;

namespace AudioBooks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureRegisterServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
          configuration.GetConnectionString("DefaultConnection") ??
          throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            object value = options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ICategoryBooksRepository, CategoryBooksRepository>();
        services.AddScoped<IUserLibraryRepository, UserLibraryRepository>();
        
        services.AddScoped(typeof(Repository<>));

        services.AddScoped<IUserService, UserService>();
        //services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserLibraryService,UserLibraryService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IUserPreferenceService, UserPreferenceService>();
        //services.AddScoped<IEmailService,EmailService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnection>(_ => new SqlConnection(connectionString));

        return services;
    }
}
