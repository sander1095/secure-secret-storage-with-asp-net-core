using Azure.Identity;
using Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Setup keyvault connection with managed identity if we are running in production
// Otherwise we use dotnet user secrets locally!
builder.Host.ConfigureAppConfiguration((context, config) =>
{
    if (!context.HostingEnvironment.IsDevelopment())
    {
        var keyVaultName = context.Configuration.GetValue<string>("KeyVaultName");
        var keyVaultUri = new Uri($"{keyVaultName}.vault.azure.net");
        config.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
    }
});

var connectionString = builder.Configuration.GetConnectionString("Database4");

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