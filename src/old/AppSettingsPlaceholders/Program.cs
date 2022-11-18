using Database;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Database2");

builder.Services.AddDbContext<PizzaDb>(options => options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

MigrateDatabase(app);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());

app.Run();

static void MigrateDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<PizzaDb>().Database.Migrate();
}

/*
 * Explanation: 
 *   
 * Have a placeholder in appsettings.json and replace that during a deploy with the right value.
 * You now do not have any real secrets in your code, you can replace them during deploy!
 *
 * Take a look at what I mean here: https://github.com/sander1095/secure-secret-storage-with-asp-net-core/runs/5078224061
 * Reasons against this approach:
 *   - How do you store secrets during development or allow a developer to have different appsettings on their PC?
 *     Put them in appsettings.Development.json and hopefully not commit them? Very tricky!
 *     (Hint: Using user-secret with this approach is.. ok.
 */ 