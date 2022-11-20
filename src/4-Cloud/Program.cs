// Source: https://github.com/sander1095/secure-secret-storage-with-asp-net-core/
using Azure.Identity;
using Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Setup keyvault connection with managed identity if we are running in production
// Otherwise we use dotnet user secrets locally! 
builder.Host.ConfigureAppConfiguration((context, config) =>
{
    Console.WriteLine($"The current environment is {context.HostingEnvironment.EnvironmentName}");
    // Comment out this IF statement to grab the connectionstrings from keyvault
    if (!context.HostingEnvironment.IsDevelopment())
    {
        var keyVaultName = context.Configuration.GetValue<string>("KeyVaultName");
        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net");
        config.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
    }
});

var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<PizzaDb>(options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

AddSwagger(builder);

var app = builder.Build();

MigrateDatabase(app);

UseSwagger(app);

app.UseHttpsRedirection();

app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());
app.MapGet("/connectionstring", (IConfiguration configuration) => configuration.GetConnectionString("Database"));

app.Run();

static void UseSwagger(WebApplication app)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

static void AddSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

static void MigrateDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<PizzaDb>().Database.Migrate();
}

/*
 * 
 * PREPARE DEMO: Allow IP address to connect to database!
 * 
 * 
 * In this demo, we have a little application that can serve us a list of pizzas from a database.
 * During development, we will use user secrets, but when we deploy our code to production on an Azure App Service, it will use a managed identity to safely talk to the application
 * 
 * - Show connectionstring in user-secrets locally (data source=.;initial catalog=pizza-db; persist security info=True;Integrated Security=SSPI;trustServerCertificate=true)
 * - Show connectionstring in appsettings.json (referring to keyvault)
 * 
 * - Show azure keyvault in portal
 * - show managed identtiy being connected from app service to database
 * - Show that experimental option (you can also run your own sql query)
 */