using AudioBooks.Application.Data;
using Npgsql;
using System.Data;

namespace AudioBooks.Infrastructure.Data;

public sealed class SqlConnection : ISqlConnection
{
    private readonly string _connectionString;

    public SqlConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection ConnectionBuild()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
