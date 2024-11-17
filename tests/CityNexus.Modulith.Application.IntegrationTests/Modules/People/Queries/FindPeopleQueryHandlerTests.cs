using CityNexus.Modulith.Application.IntegrationTests.Common;
using CityNexus.Modulith.Application.Modules.People.Queries.FindPeople;
using CityNexus.Modulith.Domain.Modules.People;
using CityNexus.Modulith.Infra.Persistence.Dapper;
using CityNexus.Modulith.Infra.Persistence.EF;
using Microsoft.EntityFrameworkCore;

namespace CityNexus.Modulith.Application.IntegrationTests.Modules.People.Queries;

[Collection("IntegrationSetup")]
public sealed class FindPeopleQueryHandlerTests(IntegrationTestSetup setup) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await setup.ResetAsync();
    }

    private async Task<(FindPeopleQueryHandler, ApplicationDbContext)> MakeSut()
    {
        var applicationDbContext = new ApplicationDbContext(
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

        var results = await sut.Handle(new FindPeopleQueryHandler.Input(), CancellationToken.None);

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

        var results = await sut.Handle(new FindPeopleQueryHandler.Input(), CancellationToken.None);

        results.Items.Should().HaveCount(1);
        results.Items.First().Id.Should().Be(people.Id);
    }
}
