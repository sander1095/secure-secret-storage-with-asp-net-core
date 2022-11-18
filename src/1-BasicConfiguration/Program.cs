var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(x =>
{
    x.AddXmlFile("appsettings.xml", optional: false, reloadOnChange: true);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IConfigurationRoot>(x => builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

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
 * Showcase adding XML provider
 * Showcase getting values from differnet providers
 */ 