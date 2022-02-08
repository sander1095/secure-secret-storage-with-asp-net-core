using Database;

using Microsoft.EntityFrameworkCore;

// Before running this sample, add an environment variable called "Database3" to your system!
// To look at all your environment variables, run this in Powershell: Get-ChildItem -Path Env:
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Database3");

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
 * Use environment variables that are system-wide so none of your configuration is anywhere in your git repo!
 * Seen a lot in Kubernetes/Docker environments
 * 
 * Let's have a look at our environment variables.
 * 
 *
 * Reasons against this approach:
 *   - Setting this up is a pain..
 *   - Applications that send crash reports like sending a list of all 
 *     environment variables to the devs, possibly including your secrets!
 */ 