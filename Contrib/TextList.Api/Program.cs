using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using RecAll.Contrib.TextList.Api;
using RecAll.Contrib.TextList.Api.Services;
using RecAll.Infrastructure;
using RecAll.Infrastructure.Api;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = InitialFunctions.CreateSerilogLogger(builder.Configuration);

try {
    builder.WebHost.CaptureStartupErrors(false).ConfigureKestrel(options => {
        options.Listen(IPAddress.Any, 81,
            listenOptions => {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        options.Listen(IPAddress.Any, 80,
            listenOptions => {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });
    });

    builder.Host.UseSerilog();

    builder.Services.AddDbContext<TextListContext>(options => {
        options.UseSqlServer(builder.Configuration["TextListContext"],
            sqlServerOptionsAction => {
                sqlServerOptionsAction.MigrationsAssembly(
                    typeof(InitialFunctions).GetTypeInfo().Assembly.GetName()
                        .Name);
                sqlServerOptionsAction.EnableRetryOnFailure(15,
                    TimeSpan.FromSeconds(30), null);
            });
    });

    builder.Services.AddTransient<IIdentityService, MockIdentityService>();

    builder.Services.AddCors(options => {
        options.AddPolicy("CorsPolicy",
            builder => builder.SetIsOriginAllowed(host => true).AllowAnyMethod()
                .AllowAnyHeader().AllowCredentials());
    });

    builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.IncludeFields = true);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    builder.Services.AddOptions().Configure<ApiBehaviorOptions>(options => {
        options.InvalidModelStateResponseFactory = context =>
            new OkObjectResult(ServiceResult.CreateInvalidParameterResult(
                    new ValidationProblemDetails(context.ModelState).Errors
                        .Select(
                            p => $"{p.Key}: {string.Join(" / ", p.Value)}"))
                .ToServiceResultViewModel());
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment()) {
        app.UseSwagger();
        app.UseSwaggerUI();
    } else {
        app.UseExceptionHandler("/Error");
    }

    app.UseCors("CorsPolicy");
    app.UseRouting();

    app.UseEndpoints(endpoints => {
        endpoints.MapDefaultControllerRoute();
        endpoints.MapControllers();
    });

    var textContext = app.Services.CreateScope().ServiceProvider
        .GetService<TextListContext>();
    textContext!.Database.Migrate();

    app.Run();
    return 0;
} catch (Exception e) {
    Log.Fatal(e, "Program terminated unexpectedly ({ApplicationContext})!",
        InitialFunctions.AppName);
    return 1;
} finally {
    Log.CloseAndFlush();
}