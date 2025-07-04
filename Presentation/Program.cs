using Application;
using Infrastructure;
using Presentation;

Presentation.ConfigureServices.PrepareApi();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddWebApiServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddOpenApiDocument();

builder.Services.AddControllers();

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi();

//app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
}

app.Run();