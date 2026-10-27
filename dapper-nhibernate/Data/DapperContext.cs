using System.Data;
using Microsoft.Data.SqlClient;

namespace DapperNHibernateApi.Data;

/// <summary>
/// A small helper that creates SQL Server connections for Dapper.
/// Dapper has no DbContext - it works directly on a connection,
/// so this class keeps the connection string in one place.
/// </summary>
public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
