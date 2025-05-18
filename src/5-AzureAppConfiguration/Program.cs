// Source: https://github.com/sander1095/secure-secret-storage-with-asp-net-core/
using Azure.Identity;

using Database;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:7226", "http://localhost:5226");

// Setup Azure App Configuration connection with managed identity if we are running in production
// Otherwise we use dotnet user secrets locally!

Console.WriteLine($"The current environment is {builder.Environment.EnvironmentName}");
// Comment out this IF statement to grab the connectionstrings from keyvault
if (!builder.Environment.IsDevelopment())
{
    var appConfigurationUriRaw = builder.Configuration.GetValue<string>("AppConfigurationUri") ?? throw new Exception("Missing AppConfigurationUri");
    var appConfigurationUri = new Uri(appConfigurationUriRaw);
    builder.Configuration.AddAzureAppConfiguration(x =>
    {
        x.Connect(appConfigurationUri, new DefaultAzureCredential());

        x.ConfigureRefresh(x => x.Register("Sentinel"));
    });
}

// Azure App Configuration retrieves this from the linked Key Vault.
builder.Services.AddDbContext<PizzaDb>((sp, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("Database");
    options.UseSqlServer(connectionString);
});

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
 * App Service URL: https://as-secret-storage-presentation.azurewebsites.net/swagger
 * Resource Group: https://portal.azure.com
 *   - app configuration
 *      - secrets (highlight connectionstring reference)
 *   - app service
 *      - identity
 *   - database
 *     - say that with a query we authenticated app service
 * 
 * In this demo, we have a little application that can serve us a list of pizzas from a database.
 * During development, we will use user secrets, but when we deploy our code to production on an Azure App Service, it will use a managed identity to safely talk to the application
 * 
 * - Show connectionstring in user-secrets locally (data source=.;initial catalog=pizza-db; persist security info=True;Integrated Security=SSPI;trustServerCertificate=true)
 * - Show connectionstring in appsettings.json (referring to azure app configuration)
 * 
 * - show app service has managed identity
 * - Show app configuration in portal and connection to managed identity
 * - show managed identtiy being connected from app service to database
 */