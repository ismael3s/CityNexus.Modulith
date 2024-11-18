using CityNexus.Modulith.Application.Modules.Shared.Abstractions;
using Dapper;
using MediatR;

namespace CityNexus.Modulith.Application.Modules.People.Queries.FindPeople;

public sealed record FindPeopleQuery(int Page = 1, int PageSize = 10): IRequest<Pagination<FindPeopleQueryHandler.Output>>;
public sealed class FindPeopleQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IRequestHandler<FindPeopleQuery, Pagination<FindPeopleQueryHandler.Output>>
{
    public sealed class Output
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = default!;

        public string Email { get; init; } = default!;

        public DateTime CreatedAt { get; init; }
    }

    public async Task<Pagination<Output>> Handle(
        FindPeopleQuery input,
        CancellationToken cancellationToken = default
    )
    {
        using var connection = sqlConnectionFactory.CreateConnection();
        var skip = (input.Page - 1) * input.PageSize;
        var take = input.PageSize > 50 ? 50 : input.PageSize;
        var query = $"""
            SELECT
                id, name, email, created_at 
            FROM person
            WHERE 1=1
            ORDER BY id ASC
            OFFSET @Skip
            LIMIT @Take
            """;
        var result = await connection.QueryAsync<Output>(
            new CommandDefinition(
                query,
                parameters: new { Skip = skip, Take = take },
                cancellationToken: cancellationToken
            )
        );
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                "SELECT count(*) FROM person",
                cancellationToken: cancellationToken
            )
        );
        return Pagination<Output>.Create(Math.Abs(count / input.PageSize), input.PageSize, take, result.ToList() );
    }

}
