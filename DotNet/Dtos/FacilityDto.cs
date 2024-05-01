namespace Endpoints.Dtos;

public record FacilityDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public required string? Description { get; init; }
}
