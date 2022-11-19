// Source: https://github.com/sander1095/secure-secret-storage-with-asp-net-core/
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<ExternalApiSettings>()
    .BindConfiguration(nameof(ExternalApiSettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();

AddSwagger(builder);

var app = builder.Build();

UseSwagger(app);

app.UseHttpsRedirection();

app.MapGet("external-user-secrets", (IOptionsSnapshot<ExternalApiSettings> options) => options.Value);

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
 *  This demo will feature user secrets and my way I like to set up configuration
 * - Talk about having your secret is appsettings.json or development.json will end up leaking it into git. Also, if develope rwants to change the value for some reason locally, you must not commit it. cumbersome.
 * - Show csproj's user secrets ID
 * - Demonstrate Visual Studio user secrets first!
 * - Then demonstrate CLI (dotnet user-secrets set "ExternalApiSettings:ApiKey" "HELLO_FROM_CLI")
 * - override value that is already in appsettings.development.json, user secrets has more importance.
 * - git shows no changes.
 * - so here you store your connectionstrings, api keys during development.
 * - if you switch to this setup, make sure to chagne your current secrets!
 * 
 * - Now show the preferred setup
 */ 