using Dapper;
using Endpoints.Dtos;
using Endpoints.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Features;

public record GetFlatsByQueryResponse
{
    public required IEnumerable<FlatDto> Flats { get; init; }
}

public class GetFlatsByQuery
{
    public static async Task<Ok<GetFlatsByQueryResponse>> Generated(
        float maxPrice,
        int minCapacity,
        DateTime startDate,
        DateTime endDate,
        [FromServices] EfCoreContext context,
        [FromServices] ILogger<GetUserReservations> logger,
        CancellationToken ct)
    {
        var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        var endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

        var query = context.Flats
            .Where(f => f.DailyPricePerPerson <= maxPrice)
            .Where(f => f.Capacity >= minCapacity)
            .Where(f => !f.Reservations.Any(r => r.StartDate <= startDateUtc && r.EndDate >= endDateUtc || r.StartDate >= startDateUtc && r.StartDate < endDateUtc || r.EndDate > startDateUtc && r.EndDate <= endDateUtc))
            .Select(f => new FlatDto
            {
                Id = f.Id,
                Description = f.Description,
                DailyPricePerPerson = f.DailyPricePerPerson,
                Capacity = f.Capacity,
                BuildingId = f.BuildingId,
                FlatNumber = f.FlatNumber
            })
            .OrderBy(f => f.Id);

        logger.LogInformation("Executing query: {0}", query.ToQueryString());

        var flats = await query.ToListAsync(ct);

        return TypedResults.Ok(new GetFlatsByQueryResponse
        {
            Flats = flats
        });
    }

    public static async Task<Ok<GetFlatsByQueryResponse>> Raw(
        float maxPrice,
        int minCapacity,
        DateTime startDate,
        DateTime endDate,
        [FromServices] DapperContext context)
    {
        using var connection = context.CreateConnection();

        var flats = await connection.QueryAsync<FlatDto>(
            @"SELECT f.id AS Id, f.description AS Description, f.daily_price_per_person AS DailyPricePerPerson, f.capacity AS Capacity, b.id AS BuildingId, f.flat_number AS FlatNumber
            FROM rental.flats f
            JOIN rental.building b ON f.building_id = b.id
            WHERE f.daily_price_per_person <= @maxPrice
            AND f.capacity >= @minCapacity
            AND NOT EXISTS (
                SELECT 1 FROM rental.reservations r
                WHERE r.flat_id = f.id
                AND (r.start_date, r.end_date) OVERLAPS (@startDate, @endDate)
            )
            ORDER BY f.id;",
            new { maxPrice, minCapacity, startDate, endDate });

        return TypedResults.Ok(new GetFlatsByQueryResponse
        {   
            Flats = flats
        });
    }
}