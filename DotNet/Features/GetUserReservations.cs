using Dapper;
using Endpoints.Dtos;
using Endpoints.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Features;

// 2

public record GetUserReservationsResponse
{
    public required IEnumerable<ReservationDto> Reservations { get; init; }
}

public class GetUserReservations
{
    public static async Task<Ok<GetUserReservationsResponse>> Generated(
        long userId,
        [FromServices] EfCoreContext context,
        [FromServices] ILogger<GetUserReservations> logger,
        CancellationToken ct)
    {
        var query = context.Reservations
            .Where(r => r.ReservedById == userId)
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                GuestNumber = r.GuestNumber,
                TotalCost = r.TotalCost
            });

        var sql = query.ToQueryString();

        await File.AppendAllTextAsync("results.md", $"## {nameof(GetUserReservations)}\n\nGenerated SQL:\n\n```\n{sql}\n```\n\n", ct);

        logger.LogInformation("Executing query: {0}", sql);

        var reservations = await query.ToListAsync(ct);

        return TypedResults.Ok(new GetUserReservationsResponse
        {   
            Reservations = reservations
        });
    }

    public static async Task<Ok<GetUserReservationsResponse>> Raw(
        long userId,
        [FromServices] DapperContext context,
        [FromServices] ILogger<GetUserReservations> logger,
        CancellationToken ct)
    {
        using var connection = context.CreateConnection();

        var sql = @"SELECT r.id AS Id, r.start_date AS StartDate, r.end_date AS EndDate, r.guest_number AS GuestNumber, r.total_cost AS TotalCost
            FROM rental.reservations r
            WHERE r.reserved_by_id = @userId;";

        await File.AppendAllTextAsync("results.md", $"## {nameof(GetUserReservations)}\n\nRaw SQL:\n\n```\n{sql}\n```\n\n", ct);

        logger.LogInformation("Executing query: {0}", sql);

        var reservations = await connection.QueryAsync<ReservationDto>(sql, new { userId });

        return TypedResults.Ok(new GetUserReservationsResponse
        {   
            Reservations = reservations
        });
    }
}