using WebApplication1.Model;

namespace WebApplication1.Helpers;

using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
        Database.EnsureCreated();  
    }

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<User>().HasData(
    //         new User { Id = 1, Name = "Tom", Email = "wow@mail.ru", Password = "lolkek"}
    //     );
    // }

    public DbSet<User> Users { get; set; } = null!;
}