using Culture.Domain.Surveys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Culture.Infrastructure.Persistence.Configurations;

public sealed class SurveyTemplateConfiguration : IEntityTypeConfiguration<SurveyTemplate>
{
    public void Configure(EntityTypeBuilder<SurveyTemplate> builder)
    {
        builder.ToTable("SurveyTemplates", "surveys");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Ignore(x => x.DomainEvents);
        builder.HasMany(x => x.Questions).WithOne().HasForeignKey(x => x.SurveyTemplateId);
    }
}

public sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions", "surveys");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Text).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(32);
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class SurveySessionConfiguration : IEntityTypeConfiguration<SurveySession>
{
    public void Configure(EntityTypeBuilder<SurveySession> builder)
    {
        builder.ToTable("SurveySessions", "responses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Ignore(x => x.DomainEvents);
        builder.HasMany(x => x.Answers).WithOne().HasForeignKey(x => x.SurveySessionId);
    }
}

public sealed class SurveyAnswerConfiguration : IEntityTypeConfiguration<SurveyAnswer>
{
    public void Configure(EntityTypeBuilder<SurveyAnswer> builder)
    {
        builder.ToTable("Answers", "responses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Value).HasMaxLength(4000).IsRequired();
        builder.Ignore(x => x.DomainEvents);
    }
}
