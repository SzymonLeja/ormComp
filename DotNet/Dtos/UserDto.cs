namespace Endpoints.Dtos;

public record UserDto
{
    public required long Id { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }
}

