using CityNexus.Modulith.People.Application.Queries.FindPeople;
using CityNexus.Modulith.People.Domain.Entities.People;
using CityNexus.Modulith.People.Infra.Persistence.Dapper;
using CityNexus.Modulith.People.Infra.Persistence.EF;
using Microsoft.EntityFrameworkCore;

namespace CityNexus.Modulith.People.IntegrationTests.People.Queries;

[Collection("IntegrationSetup")]
public sealed class FindPeopleQueryHandlerTests(IntegrationTestSetup setup) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await setup.ResetAsync();
    }

    private async Task<(FindPeopleQueryHandler, PersonDbContext)> MakeSut()
    {
        var applicationDbContext = new PersonDbContext(
            new DbContextOptionsBuilder()
                .UseSnakeCaseNamingConvention()
                .UseNpgsql(setup.ConnectionString)
                .Options
        );
        await applicationDbContext.Database.EnsureCreatedAsync();
        var sqlConnectionFactory = new SqlConnectionFactory(setup.ConnectionString);
        var sut = new FindPeopleQueryHandler(sqlConnectionFactory);
        return (sut, applicationDbContext);
    }

    [Fact]
    [Trait(
        "FindPeople - Integration",
        "Should return an empty pagination when there is no results"
    )]
    public async Task FindPeople_ShouldReturnNothingWhenThereIsNoPeople()
    {
        var (sut, _) = await MakeSut();

        var results = await sut.Handle(new FindPeopleQuery(), CancellationToken.None);

        results.Items.Should().BeEmpty();
    }

    [Fact]
    [Trait("FindPeople - Integration", "Should return the results from the database")]
    public async Task FindPeople_ShouldReturnTheResultsWhenThereIsPeopleRegistered()
    {
        var (sut, applicationDbContext) = await MakeSut();
        var people = Person.Create("John Doe", "doe@gmail.com", "493.234.700-66");
        applicationDbContext.People.Add(people);
        await applicationDbContext.SaveChangesAsync();

        var results = await sut.Handle(new FindPeopleQuery(), CancellationToken.None);

        results.Items.Should().HaveCount(1);
        results.Items.First().Id.Should().Be(people.Id);
    }
}
