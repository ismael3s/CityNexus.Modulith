using System.Data;

namespace CityNexus.Modulith.SharedKernel.Abstractions;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
