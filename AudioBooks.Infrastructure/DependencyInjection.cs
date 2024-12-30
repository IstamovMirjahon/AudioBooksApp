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
        services.AddScoped(typeof(Repository<>));

        services.AddScoped<IUserService, UserService>();


        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnection>(_ => new SqlConnection(connectionString));

        return services;
    }
}
