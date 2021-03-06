using Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Database1");

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
 * - A simple application showing you the danger of having secrets in your git repo. 
 *   We have a connectionstring to a database which returns a list of pizzas.
 *   We use swagger (OpenAPI) to make this easy to test on /swagger
 *   
 * - Take a look at appsettings.json and appsettings.Development.json. 
 *   In launchsettings you can see the environment is Development, 
 *   so that is used on top of appsettings.json 
 *   
 * Reasons against this approach:
 * - Secrets are in your git repo! They can't be deleted after you commit and push it.
 */ 