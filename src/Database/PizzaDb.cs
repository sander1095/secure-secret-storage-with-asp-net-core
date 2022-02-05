using Microsoft.EntityFrameworkCore;

namespace Database;

public class PizzaDb : DbContext
{
    public PizzaDb(DbContextOptions<PizzaDb> options) : base(options) { }

    public DbSet<Pizza> Pizzas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Pizza>().HasData(
            new Pizza { Id = 1, Name = "Salami" },
            new Pizza { Id = 2, Name = "Hawaii" },
            new Pizza { Id = 3, Name = "Margherita" });

    }
}
