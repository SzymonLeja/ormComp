using Dapper;
using Endpoints.Dtos;
using Endpoints.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Features;

public record GetFlatsNearLocationResponse
{
    public required IEnumerable<FlatDto> Flats { get; init; }
}

public class GetFlatsNearLocation
{
    public static async Task<Ok<GetFlatsNearLocationResponse>> Generated(
        double latitude,
        double longitude,
        double radius,
        [FromServices] EfCoreContext context,
        [FromServices] ILogger<GetFlatsNearLocation> logger,
        CancellationToken ct)
    {
        var query = context.Flats
            .Where(f => 6371 * Math.Acos(
                Math.Cos(Math.PI * latitude / 180) * Math.Cos(Math.PI * (double)f.Building.Address.Latitude / 180) * Math.Cos(Math.PI * (double)f.Building.Address.Longitude / 180 - Math.PI * longitude / 180) +
                Math.Sin(Math.PI * latitude / 180) * Math.Sin(Math.PI * (double)f.Building.Address.Latitude / 180)
            ) < radius)
            .Select(f => new FlatDto
            {
                Id = f.Id,
                Description = f.Description,
                DailyPricePerPerson = f.DailyPricePerPerson,
                Capacity = f.Capacity,
                BuildingId = f.BuildingId,
                FlatNumber = f.FlatNumber
            });

        var sql = query.ToQueryString();

        await File.AppendAllTextAsync("results.md", $"## {nameof(GetFlatsNearLocation)}\n\nGenerated SQL:\n\n```\n{sql}\n```\n\n", ct);

        logger.LogInformation("Executing query: {0}", sql);

        var flats = await query.ToListAsync(ct);

        return TypedResults.Ok(new GetFlatsNearLocationResponse
        {
            Flats = flats
        });
    }

    public static async Task<Ok<GetFlatsNearLocationResponse>> Raw(
        float latitude,
        float longitude,
        float radius,
        [FromServices] DapperContext context,
        [FromServices] ILogger<GetFlatsNearLocation> logger,
        CancellationToken ct)
    {
        using var connection = context.CreateConnection();

        var sql = @"
            SELECT f.id AS Id, f.description AS Description, f.daily_price_per_person AS DailyPricePerPerson, f.capacity AS Capacity, f.building_id AS BuildingId, f.flat_number AS FlatNumber
            FROM rental.flats f
            JOIN rental.building b ON f.building_id = b.id
            JOIN rental.addresses a ON b.address_id = a.id
            WHERE (
                6371 * acos (
                    cos(radians(@latitude)) * cos(radians(a.latitude)) * cos(radians(a.longitude - @longitude)) +
                    sin(radians(@latitude)) * sin(radians(a.latitude))
                )
            ) < @radius;";

        await File.AppendAllTextAsync("results.md", $"## {nameof(GetFlatsNearLocation)}\n\nRaw SQL:\n\n```\n{sql}\n```\n\n", ct);

        logger.LogInformation("Executing query: {0}", sql);

        var flats = await connection.QueryAsync<FlatDto>(sql, new { latitude, longitude, radius });

        return TypedResults.Ok(new GetFlatsNearLocationResponse
        {
            Flats = flats
        });
    }
}