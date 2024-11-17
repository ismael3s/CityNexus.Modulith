using System.Data;
using CityNexus.Modulith.Application.Modules.Shared.Abstractions;
using Npgsql;

namespace CityNexus.Modulith.Infra.Persistence.Dapper;

public sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}
