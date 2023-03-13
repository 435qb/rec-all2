using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RecAll.Contrib.TextList.Api;
using RecAll.Contrib.TextList.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TextListContext>(options => {
    options.UseSqlServer(builder.Configuration["TextListContext"],
        sqlServerOptionsAction => {
            sqlServerOptionsAction.MigrationsAssembly(typeof(InitialFunctions)
                .GetTypeInfo().Assembly.GetName().Name);
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