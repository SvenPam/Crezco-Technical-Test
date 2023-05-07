using Crezco.Application;
using Crezco.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
Crezco.Infrastructure.Registration.EnsurePersistenceCreated(serviceScope);

app.Run();

namespace Crezco.API
{
    /// <summary>
    ///     This exists purely for integration tests.
    /// </summary>
    public class Program
    {
    }
}