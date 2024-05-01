namespace Endpoints.Dtos;

public record FlatDto
{
    public required long Id { get; init; }

    public required string? Description { get; init; }

    public required short Capacity { get; init; }

    public required float DailyPricePerPerson { get; init; }

    public required string? FlatNumber { get; init; }

    public required long BuildingId { get; init; }
}
