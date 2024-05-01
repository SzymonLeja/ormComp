namespace Endpoints.Dtos;

public record ReservationDto
{
    public required long Id { get; init; }

    public required DateTime StartDate { get; init; }

    public required DateTime EndDate { get; init; }

    public required short GuestNumber { get; init; }

    public required float TotalCost { get; init; }
}
