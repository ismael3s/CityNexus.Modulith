using System.Data;

namespace CityNexus.Modulith.Application.Modules.Shared.Abstractions;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
