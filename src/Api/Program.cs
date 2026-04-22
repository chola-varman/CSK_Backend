using CskMasala.Email.Worker;
using CskMasala.Identity.Api;
using CskMasala.Identity.Infrastructure;
using CskMasala.Payment.Api;
using CskMasala.Payment.Infrastructure;
using CskMasala.Receipt.Api;
using CskMasala.Receipt.Application;
using CskMasala.Retail.Api;
using CskMasala.Retail.Infrastructure;
using CskMasala.Shared;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, services, config) =>
    {
        config
            .ReadFrom.Configuration(ctx.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "CskMasala")
            .WriteTo.Console()
            .WriteTo.File("logs/cskmasala-.log", rollingInterval: RollingInterval.Day);
    });

    builder.Services.AddShared(builder.Configuration);

    builder.Services.AddIdentityModule(builder.Configuration);
    builder.Services.AddRetailModule(builder.Configuration);
    builder.Services.AddPaymentModule(builder.Configuration);
    builder.Services.AddReceiptModule();
    builder.Services.AddEmailWorker();

    builder.Services
        .AddControllers()
        .AddIdentityControllers()
        .AddRetailControllers()
        .AddPaymentControllers()
        .AddReceiptControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "CSK Masala API",
            Version = "v1",
            Description = "Retail backend for CSK Masala products"
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "Firebase JWT",
            In = ParameterLocation.Header,
            Description = "Paste your Firebase ID token here"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });

        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddHealthChecks();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "CSK Masala v1");
            options.RoutePrefix = "swagger";
        });
    }

    app.UseSharedMiddleware();
    app.UseFirebaseAuth();
    app.UseHttpMetrics();
    app.UseRouting();
    app.UseAuthorization();
    app.MapControllers();
    app.MapMetrics();
    app.MapHealthChecks("/health");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
