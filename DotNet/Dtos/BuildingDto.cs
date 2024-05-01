namespace Endpoints.Dtos;

public record BuildingDto
{
    public required long Id { get; init; }

    public required string? Description { get; init; }

    public required AddressDto Address { get; init; }

    public required IEnumerable<FlatDto> Flats { get; init; }
}
