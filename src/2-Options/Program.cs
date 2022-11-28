// Source: https://github.com/sander1095/secure-secret-storage-with-asp-net-core/

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

AddSwagger(builder);

// Simple version:
//builder.Services.Configure<ExternalApiSettings>(builder.Configuration);


builder.Services
    .AddOptions<ExternalApiSettings>()
    .BindConfiguration(nameof(ExternalApiSettings))
    .ValidateDataAnnotations()
    .Validate(x =>
    {
        // You can even do your own complex validation!
        // You can also inject dependencies into this function!
        return x.ApiUrl != "https://stenbrinke.nl";
    })
    .ValidateOnStart();

// BackgroundJob for options monitor
builder.Services.AddHostedService<BackgroundJob>();
var app = builder.Build();

UseSwagger(app);

app.MapGet("options", (IOptions<ExternalApiSettings> options) => options.Value);
app.MapGet("options-snapshot", (IOptionsSnapshot<ExternalApiSettings> options) => options.Value);
app.MapGet("options-monitor", (IOptionsMonitor<ExternalApiSettings> options) => options.CurrentValue);

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

public class ExternalApiSettings
{
    [Required]
    public string ApiUrl { get; set; } = null!;

    [Required]
    public string ApiKey { get; set; } = null!;

    [Range(1, 1_000_00)]
    public int TimeoutInMilliseconds { get; set; }
}

/*
 * 
 * - Demonstrate the ExternalApiSettings in the appsettings and show the class
 * - Show the easy commented out variant with Configure.
 * - Show the validate code Change appsettings, demonstrate validation fail on startup, otherwise it would only fail when being accessed the first time
 * - Demonstrate options. Show that if you change a value, it doesn't update.
 * - Show snapshot, which does work!
 * - OptionsMonitor looks the same but has other functionality, look at the backgroundjob and then at the console
 * - Show that if you change your options to invalid value, the code will not call the optionsmonitor change event and options snapshot will throw an error when retrieving
 */ 