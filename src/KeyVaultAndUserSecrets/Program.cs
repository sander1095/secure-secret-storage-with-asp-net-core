
using Azure.Identity;

using Database;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Setup keyvault connection with managed identity if we are running in production
// Otherwise we use dotnet user secrets locally! 
builder.Host.ConfigureAppConfiguration((context, config) =>
{
    // Comment out this IF statement to grab the connectionstrings/api key from keyvault
    if (!context.HostingEnvironment.IsDevelopment())
    {
        var keyVaultName = context.Configuration.GetValue<string>("KeyVaultName");
        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net");
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
app.MapGet("/keyvault", (IConfiguration configuration) => configuration.GetValue<string>("SomeExternalApi:ApiKey"));

app.Run();

static void MigrateDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<PizzaDb>().Database.Migrate();
}