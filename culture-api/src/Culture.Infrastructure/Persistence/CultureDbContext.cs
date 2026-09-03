using Culture.Application.Abstractions;
using Culture.Domain.Activities;
using Culture.Domain.Identity;
using Culture.Domain.Surveys;
using Microsoft.EntityFrameworkCore;

namespace Culture.Infrastructure.Persistence;

public sealed class CultureDbContext(DbContextOptions<CultureDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<Activity> Activities => Set<Activity>();

    public DbSet<ActivityAssignment> ActivityAssignments => Set<ActivityAssignment>();

    public DbSet<Buddy> Buddies => Set<Buddy>();

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    public DbSet<SurveyTemplate> SurveyTemplates => Set<SurveyTemplate>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<SurveySession> SurveySessions => Set<SurveySession>();

    public DbSet<SurveyAnswer> SurveyAnswers => Set<SurveyAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CultureDbContext).Assembly);
    }
}
