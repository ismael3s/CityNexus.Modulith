using System.Data.Common;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace CityNexus.Modulith.People.IntegrationTests;

public class IntegrationTestSetup : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresqlContainer = new PostgreSqlBuilder().Build();
    private Respawner _respawner = default!;
    private DbConnection _dbConnection = default!;
    public string ConnectionString = default!;

    public async Task InitializeAsync()
    {
        await _postgresqlContainer.StartAsync();
        ConnectionString = _postgresqlContainer.GetConnectionString();
        _dbConnection = new NpgsqlConnection(_postgresqlContainer.GetConnectionString());
        await _dbConnection.OpenAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _postgresqlContainer.DisposeAsync().AsTask();
    }

    public async Task ResetAsync()
    {
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions { DbAdapter = DbAdapter.Postgres, SchemasToInclude = ["public"] }
        );
        await _respawner.ResetAsync(_dbConnection);
    }
}

[CollectionDefinition("IntegrationSetup")]
public class IntegrationSetup : ICollectionFixture<IntegrationTestSetup> { }
