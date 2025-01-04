using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.Models.BookModels;
using AudioBooks.Domain.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace AudioBooks.Infrastructure;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookCategory>()
           .HasKey(bc => new { bc.BookId, bc.CategoryId });

        modelBuilder.Entity<BookCategory>()
            .HasOne(bc => bc.Book)
            .WithMany(b => b.BookCategories)
            .HasForeignKey(bc => bc.BookId);

        modelBuilder.Entity<BookCategory>()
            .HasOne(bc => bc.Category)
            .WithMany(c => c.BookCategories)
            .HasForeignKey(bc => bc.CategoryId);

        modelBuilder.Entity<UserPreference>()
            .HasOne(up => up.User)
            .WithMany()
            .HasForeignKey(up => up.UserId);

        modelBuilder.Entity<UserPreference>()
            .HasOne(up => up.Category)
            .WithMany()
            .HasForeignKey(up => up.CategoryId);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Book)
            .WithMany(b => b.Comments)
            .HasForeignKey(c => c.BookId);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

    public DbSet<User> Users { get; set; }

    //Category
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BookCategory> BookCategories { get; set; }

    public DbSet<UserLibrary> userLibraries { get; set; }

    public DbSet<UserPreference> UserPreferences { get; set; }

    public DbSet<Comment> Comments { get; set; }
}
