using Culture.Domain.Activities;
using Culture.Infrastructure.Persistence;
using NetArchTest.Rules;

namespace Culture.ArchitectureTests;

public sealed class LayerDependencyTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        TestResult result = Types.InAssembly(typeof(Activity).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Culture.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        TestResult result = Types.InAssembly(typeof(CultureDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Culture.Api")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
