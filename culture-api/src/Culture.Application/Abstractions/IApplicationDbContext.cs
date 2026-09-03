using Culture.Domain.Activities;
using Culture.Domain.Identity;
using Culture.Domain.Surveys;
using Microsoft.EntityFrameworkCore;

namespace Culture.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Activity> Activities { get; }

    DbSet<ActivityAssignment> ActivityAssignments { get; }

    DbSet<Buddy> Buddies { get; }

    DbSet<AdminUser> AdminUsers { get; }

    DbSet<SurveyTemplate> SurveyTemplates { get; }

    DbSet<Question> Questions { get; }

    DbSet<SurveySession> SurveySessions { get; }

    DbSet<SurveyAnswer> SurveyAnswers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
