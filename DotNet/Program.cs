using Endpoints.Features;
using Endpoints.Middleware;
using Endpoints.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.CustomSchemaIds(x => x.FullName);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials());
});

builder.Services.AddDbContext<EfCoreContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RentalDbContext"), x => x.UseNetTopologySuite()));

builder.Services.AddScoped<DapperContext>();

builder.Services.AddScoped<RequestTimeMiddleware>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<RequestTimeMiddleware>();

app.UseCors("CorsPolicy");

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.MapGet("/api/users/{userId:long}/flats/best", GetUserFlatWithHighestIncome.Generated);
app.MapGet("/api/users/{userId:long}/flats/best/raw", GetUserFlatWithHighestIncome.Raw);

app.MapGet("/api/users/{userId:long}/reservations", GetUserReservations.Generated);
app.MapGet("/api/users/{userId:long}/reservations/raw", GetUserReservations.Raw);

app.MapGet("/api/users/query", GetUsersWithFlatNumberInCity.Generated);
app.MapGet("/api/users/query/raw", GetUsersWithFlatNumberInCity.Raw);

app.MapGet("/api/flats/stats", GetFlatsByCapacityAndRevenue.Generated);
app.MapGet("/api/flats/stats/raw", GetFlatsByCapacityAndRevenue.Raw);

app.MapGet("/api/flats/query", GetFlatsByQuery.Generated);
app.MapGet("/api/flats/query/raw", GetFlatsByQuery.Raw);

app.MapGet("/api/flats/popular", GetMostPopularFlatStats.Generated);
app.MapGet("/api/flats/popular/raw", GetMostPopularFlatStats.Raw);

app.MapGet("/api/flats/search", GetFlatsNearLocation.Generated);
app.MapGet("/api/flats/search/raw", GetFlatsNearLocation.Raw);

app.Run();