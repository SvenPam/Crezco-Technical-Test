using Crezco.API.Filters;
using Crezco.Application;
using Crezco.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;
using Registration = Crezco.Infrastructure.Registration;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(o => o.Filters.Add(typeof(ResponseFilter)));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Crezco Technical Test", Version = "v1", Description = "Ste Pammenter submission."});

    // Set the comments path for the Swagger JSON and UI.
    var basePath = AppContext.BaseDirectory;
    var xmlPath = Path.Combine(basePath, $@"{System.AppDomain.CurrentDomain.BaseDirectory}\Crezco.API.xml");
    c.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<CosmosConfiguration>(
    builder.Configuration.GetSection(nameof(CosmosConfiguration)));

builder.Services.AddApplicationServices();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Configuration.GetSection(nameof(CosmosConfiguration)).Bind(new CosmosConfiguration());

using var serviceScope = app.Services.CreateScope();
Registration.EnsurePersistenceCreated(serviceScope);

app.Run();

namespace Crezco.API
{
    /// <summary>
    ///     This exists purely for integration tests.
    /// </summary>
    public partial class Program
    {
    }
}