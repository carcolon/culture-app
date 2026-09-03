using Culture.Application.Abstractions;
using Culture.Application.Identity;
using Culture.Domain.Identity;
using Culture.Infrastructure.Identity;
using Culture.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Culture.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string provider = configuration["Database:Provider"] ?? "SqlServer";
        string connectionString = configuration.GetConnectionString("CultureDb")
            ?? "Server=(localdb)\\mssqllocaldb;Database=CultureDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        services.AddDbContext<CultureDbContext>(options =>
        {
            if (string.Equals(provider, "InMemory", StringComparison.OrdinalIgnoreCase))
            {
                options.UseInMemoryDatabase("CultureDb");
                return;
            }

            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<CultureDbContext>());
        services.Configure<BuddyAuthenticationOptions>(configuration.GetSection("Security:BuddyAuth"));
        services.AddScoped<PasswordHasher<Buddy>>();
        services.AddScoped<PasswordHasher<AdminUser>>();
        services.AddScoped<IBuddyAuthenticationService, BuddyAuthenticationService>();
        services.AddScoped<IAdminAuthenticationService, AdminAuthenticationService>();

        return services;
    }
}
