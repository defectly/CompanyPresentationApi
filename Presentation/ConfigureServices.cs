using Application;
using AutoMapper;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Presentation.Common;
using System.Text.Json.Serialization;

namespace Presentation;

public static class ConfigureServices
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>())
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        return services;
    }

    public static void PrepareApi()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Configuration.AddEnvironmentVariables();

        builder.Services
            .AddWebApiServices(builder.Configuration)
            .AddInfrastructure(builder.Configuration)
            .AddApplication(builder.Configuration);

        using var app = builder.Build();
        using var scope = app.Services.CreateScope();

        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        var db = scope.ServiceProvider.GetRequiredService<CompanyDbContext>();

        if (db.Database.IsSqlServer())
            db.Database.Migrate();
    }
}
