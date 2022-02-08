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

/*
 * Explanation: 
 *   
 * Use the power of the cloud™️ to store your secrets somewhere safe! Also super cheap! (3 cents for 10000 calls)
 * At runtime we add a new configuration provider (KeyVault) so we can simply ask for the connectionstring from the KeyVault!
 * You could also look at other options like Hashicorp Vault, AWS Secret Manager, etc..
 * 
 * During development we use user-secrets to store our secrets safely (but not encrypted!!!) away from our code.
 * It would be safer to use KeyVault here as well, but then you require an internet connection... so it depends on your scenario.
 * 
 * You can also see the power of managed identities so we can authenticate azure to azure resources without passwords! Neat!
 * 
 */ 