using Dapper;
using Endpoints.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Features;

public record GetFlatsByCapacityAndRevenueResponse
{
    public required IEnumerable<GetFlatsByCapacityAndRevenueStatDto> Stats { get; init; }
}

public record GetFlatsByCapacityAndRevenueStatDto
{
    public required long NumberOfRentals { get; init; }

    public required short Capacity { get; init; }

    public required float TotalRevenue { get; init; }

}

public class GetFlatsByCapacityAndRevenue
{
    public static async Task<Ok<GetFlatsByCapacityAndRevenueResponse>> Generated(
        [FromServices] EfCoreContext context,
        [FromServices] ILogger<GetFlatsByCapacityAndRevenue> logger,
        CancellationToken ct)
    {
        var query = context.Flats
            .GroupBy(f => f.Capacity)
            .Select(g => new GetFlatsByCapacityAndRevenueStatDto
            {
                Capacity = g.Key,
                NumberOfRentals = g.SelectMany(f => f.Reservations).Count(),
                TotalRevenue = g.Sum(f => f.Reservations.Sum(r => r.TotalCost))
            })
            .OrderByDescending(f => f.NumberOfRentals)
            .ThenByDescending(f => f.TotalRevenue);

        logger.LogInformation("Executing query: {0}", query.ToQueryString());

        var flats = await query.ToListAsync(ct);

        return TypedResults.Ok(new GetFlatsByCapacityAndRevenueResponse
        {
            Stats = flats
        });
    }

    public static async Task<Ok<GetFlatsByCapacityAndRevenueResponse>> Raw(
        [FromServices] DapperContext context)
    {
        using var connection = context.CreateConnection();

        var stats = await connection.QueryAsync<GetFlatsByCapacityAndRevenueStatDto>(
            @"SELECT f.capacity AS Capacity, COUNT(r.id) AS NumberOfRentals, SUM(r.total_cost) AS TotalRevenue
            FROM rental.flats f
            JOIN rental.reservations r ON f.id = r.flat_id
            GROUP BY f.capacity
            ORDER BY NumberOfRentals DESC, TotalRevenue DESC;");

        return TypedResults.Ok(new GetFlatsByCapacityAndRevenueResponse
        {   
            Stats = stats
        });
    }
}