using Dapper;
using Endpoints.Dtos;
using Endpoints.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Features;

// 3

public record GetUsersWithFlatNumberInCityResponse
{
    public required IEnumerable<UserDto> Users { get; init; }
}

public class GetUsersWithFlatNumberInCity
{
    public static async Task<Ok<GetUsersWithFlatNumberInCityResponse>> Generated(
        string city,
        int flatNumber,
        [FromServices] EfCoreContext context,
        [FromServices] ILogger<GetUsersWithFlatNumberInCity> logger,
        CancellationToken ct)
    {
        var query = context.Users
            .Where(u => u.Buildings
                .Where(b => b.Address.City == city)
                .SelectMany(b => b.Flats)
                .Count() == flatNumber)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName
            });

        var sql = query.ToQueryString();

        await File.AppendAllTextAsync("results.md", $"## {nameof(GetUsersWithFlatNumberInCity)}\n\nGenerated SQL:\n\n```\n{sql}\n```\n\n", ct);

        logger.LogInformation("Executing query: {0}", sql);

        var users = await query.ToListAsync(ct);

        return TypedResults.Ok(new GetUsersWithFlatNumberInCityResponse
        {
            Users = users
        });
    }

    public static async Task<Ok<GetUsersWithFlatNumberInCityResponse>> Raw(
        string city,
        int flatNumber,
        [FromServices] DapperContext context,
        [FromServices] ILogger<GetUsersWithFlatNumberInCity> logger,
        CancellationToken ct)
    {
        using var connection = context.CreateConnection();

        var sql = @"
            SELECT u.id AS Id, u.first_name AS FirstName, u.last_name AS LastName
            FROM rental.users u
            JOIN rental.building b ON u.id = b.owner_id
            JOIN rental.flats f ON b.id = f.building_id
            JOIN rental.addresses a ON b.address_id = a.id
            WHERE a.city = @city
            GROUP BY u.id
            HAVING COUNT(f.id) = @flatNumber;";

        await File.AppendAllTextAsync("results.md", $"## {nameof(GetUsersWithFlatNumberInCity)}\n\nRaw SQL:\n\n```\n{sql}\n```\n\n", ct);

        logger.LogInformation("Generated SQL: {0}", sql);

        var users = await connection.QueryAsync<UserDto>(sql, new { city, flatNumber });

        return TypedResults.Ok(new GetUsersWithFlatNumberInCityResponse
        {
            Users = users
        });
    }
}