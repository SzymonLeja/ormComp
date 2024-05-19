using Dapper;
using Endpoints.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Features;

// 5

public record GetMostPopularFlatStatsResponse
{
    public required IEnumerable<GetMostPopularFlatStatDto> Stats { get; init; }
}

public record GetMostPopularFlatStatDto
{
    public required string Amenity { get; init; }

    public required int RentalCount { get; init; }
}

public class GetMostPopularFlatStats
{
    public static async Task<Ok<GetMostPopularFlatStatsResponse>> Generated(
        string city,
        [FromServices] EfCoreContext context,
        [FromServices] ILogger<GetMostPopularFlatStats> logger,
        CancellationToken ct)
    {
        var query = context.Reservations
            .Where(r => r.Flat.Building.Address.City == city)
            .SelectMany(r => r.Flat.Facilities)
            .GroupBy(f => f.Name)
            .Select(g => new GetMostPopularFlatStatDto
            {
                Amenity = g.Key,
                RentalCount = g.Count()
            })
            .OrderByDescending(f => f.RentalCount);

        var sql = query.ToQueryString();

        await File.AppendAllTextAsync("results.md", $"## {nameof(GetMostPopularFlatStats)}\n\nGenerated SQL:\n\n```\n{sql}\n```\n\n", ct);

        logger.LogInformation("Executing query: {0}", sql);

        var stats = await query.ToListAsync(ct);

        return TypedResults.Ok(new GetMostPopularFlatStatsResponse
        {
            Stats = stats
        });
    }

    public static async Task<Ok<GetMostPopularFlatStatsResponse>> Raw(
        string city,
        [FromServices] DapperContext context,
        [FromServices] ILogger<GetMostPopularFlatStats> logger,
        CancellationToken ct)
    {
        using var connection = context.CreateConnection();

        var sql = @"SELECT fac.name AS Amenity, COUNT(*) AS RentalCount
            FROM rental.reservations r
            JOIN rental.flats f ON r.flat_id = f.id
            JOIN rental.building b ON f.building_id = b.id
            JOIN rental.addresses a ON b.address_id = a.id
            JOIN rental.flat_facility ff ON f.id = ff.flat_id
            JOIN rental.facilities fac ON ff.facility_id = fac.id
            WHERE a.city = @city
            GROUP BY fac.name
            ORDER BY RentalCount DESC;";

        await File.AppendAllTextAsync("results.md", $"## {nameof(GetMostPopularFlatStats)}\n\nRaw SQL:\n\n```\n{sql}\n```\n\n", ct);

        logger.LogInformation("Executing query: {0}", sql);

        var stats = await connection.QueryAsync<GetMostPopularFlatStatDto>(sql, new { city });

        return TypedResults.Ok(new GetMostPopularFlatStatsResponse
        {
            Stats = stats
        });
    }
}