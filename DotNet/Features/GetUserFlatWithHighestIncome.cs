using Dapper;
using Endpoints.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Features;

// 6

public record GetUserFlatWithHighestIncomeResponse
{
    public required long Id { get; init; }
    public required float TotalRevenue { get; init; }
};

public class GetUserFlatWithHighestIncome
{
    public static async Task<Ok<GetUserFlatWithHighestIncomeResponse>> Generated(
        long userId,
        DateTime startDate,
        DateTime endDate,
        [FromServices] EfCoreContext context,
        [FromServices] ILogger<GetUserFlatWithHighestIncome> logger,
        CancellationToken ct)
    {
        var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        var endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

        var query = context.Flats
            .Where(f => f.Building.OwnerId == userId)
            .Select(f => new GetUserFlatWithHighestIncomeResponse
            {
                Id = f.Id,
                TotalRevenue = f.Reservations
                    .Where(r => r.StartDate >= startDate.ToUniversalTime() && r.EndDate <= endDate.ToUniversalTime())
                    .Sum(r => r.TotalCost)
            })
            .OrderByDescending(f => f.TotalRevenue);

        var sql = query.ToQueryString();

        await File.AppendAllTextAsync("results.md", $"## {nameof(GetUserFlatWithHighestIncome)}\n\nGenerated SQL:\n\n```\n{sql}\n```\n\n", ct);

        logger.LogInformation("Executing query: {0}", sql);

        var flat = await query.FirstOrDefaultAsync(ct);

        return TypedResults.Ok(flat);
    }

    public static async Task<Ok<GetUserFlatWithHighestIncomeResponse>> Raw(
        long userId,
        DateTime startDate,
        DateTime endDate,
        [FromServices] DapperContext context,
        [FromServices] ILogger<GetUserFlatWithHighestIncome> logger,
        CancellationToken ct)
    {
        using var connection = context.CreateConnection();

        var sql = @"
            SELECT f.id AS Id, SUM(r.total_cost) AS TotalRevenue
            FROM rental.flats f
            JOIN rental.reservations r ON f.id = r.flat_id
            WHERE r.start_date >= @startDate AND r.end_date <= @endDate
            AND f.building_id IN (SELECT id FROM rental.building WHERE owner_id = @userId) 
            GROUP BY f.id
            ORDER BY TotalRevenue DESC
            LIMIT 1;";
        
        await File.AppendAllTextAsync("results.md", $"## {nameof(GetUserFlatWithHighestIncome)}\n\nGenerated SQL:\n\n```\n{sql}\n```\n\n", ct);

        logger.LogInformation("Executing query: {0}", sql);

        var flat = await connection.QueryFirstOrDefaultAsync<GetUserFlatWithHighestIncomeResponse>(sql, new { userId, startDate, endDate });

        return TypedResults.Ok(flat);
    }
}