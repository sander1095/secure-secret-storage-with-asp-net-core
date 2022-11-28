// Source: https://github.com/sander1095/secure-secret-storage-with-asp-net-core/

var builder = WebApplication.CreateBuilder(args);

// Adding our own XmlFile provider
builder.Host.ConfigureAppConfiguration(x =>
{
    x.AddXmlFile("appsettings.xml", optional: false, reloadOnChange: true);
});

AddSwagger(builder);

builder.Services.AddTransient<IConfigurationRoot>(x => builder.Configuration);

var app = builder.Build();

UseSwagger(app);

// When using WebApplication.CreateBuilder(args), some default configuration providers are set up for you. This endpoint will return them!
app.MapGet("/default-configuration", (IConfigurationRoot configurationRoot) => configurationRoot.Providers.Take(8).Select(x => x.ToString()));

// This endpoint will return various values from configuration.
app.MapGet("/various-configuration-values", (IConfiguration configuration) => new
{
    Cli = configuration.GetValue<string>("clikey"),
    Xml = configuration.GetValue<string>("xmlkey"),
    Env = configuration.GetValue<string>("environmentkey"),
    AppSettings = configuration.GetValue<string>("SomeHierarchy:AnotherObject:WelcomeText") // Using ':' to go deeper into a configuration hierarchy
});

app.UseHttpsRedirection();
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


/*
 * Demo steps:
 * Minimal API, some endpoints to demonstrate basic configuration
 * Showcase that our env is development because of launchsettings
 * Showcase appsettings files
 * Showcase XML file (non default)
 * Showcase CLI argument (launchsettings)
 * Showcase Environment variable(launchsettings)
 * 
 * Let's showcase the default configuration providers
 * Showcase normal configuration
 * Showcase getting values from differnet providers
 * Showcase adding XML provider with reloadonchange
 */ 