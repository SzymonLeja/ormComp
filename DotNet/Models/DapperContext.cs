
using System.Data;
using Npgsql;

namespace Endpoints.Models;

public class DapperContext
{
    private readonly string _connectionString;
    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("RentalDbContext")!;
    }
    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);
}