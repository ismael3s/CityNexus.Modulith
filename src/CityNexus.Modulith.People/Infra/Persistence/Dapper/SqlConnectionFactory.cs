using System.Data;
using CityNexus.Modulith.SharedKernel.Abstractions;
using Npgsql;

namespace CityNexus.Modulith.People.Infra.Persistence.Dapper;

public sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}
