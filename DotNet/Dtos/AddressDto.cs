namespace Endpoints.Dtos;

public record AddressDto
{
    public required long Id { get; init; }

    public required string City { get; init; }

    public required string Number { get; init; }

    public required string Street { get; init; }

    public required string PostCode { get; init; }

    public required string Country { get; init; }

    public required decimal Longitude { get; init; }

    public required decimal Latitude { get; init; }
}
