
using Microsoft.EntityFrameworkCore;
using TheBabyCowboy.ContentApi.Models;

namespace TheBabyCowboy.ContentApi.Data;

public class ContentDbContext : DbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<SkillCategory> SkillCategories => Set<SkillCategory>();

    public DbSet<Skill> Skills => Set<Skill>();

    public DbSet<ProjectSkill> ProjectSkills => Set<ProjectSkill>();

    public DbSet<Certification> Certifications => Set<Certification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("ContentProjects");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(p => p.ShortDescription)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(p => p.Description)
                .IsRequired();

            entity.Property(p => p.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(p => p.ThumbnailUrl)
                .HasMaxLength(2048);

            entity.Property(p => p.GitHubUrl)
                .HasMaxLength(2048);
        });

        modelBuilder.Entity<SkillCategory>(entity =>
        {
            entity.ToTable("ContentSkillCategories");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(c => c.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.ToTable("ContentSkills");

            entity.HasKey(s => s.Id);

            entity.Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(s => s.IconUrl)
                .HasMaxLength(2048);

            entity.HasOne(s => s.SkillCategory)
                .WithMany(c => c.Skills)
                .HasForeignKey(s => s.SkillCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(s => new { s.SkillCategoryId, s.Name })
                .IsUnique();
        });

        modelBuilder.Entity<ProjectSkill>(entity =>
        {
            entity.ToTable("ContentProjectSkills");

            entity.HasKey(ps => new { ps.ProjectId, ps.SkillId });

            entity.HasOne(ps => ps.Project)
                .WithMany(p => p.ProjectSkills)
                .HasForeignKey(ps => ps.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ps => ps.Skill)
                .WithMany(s => s.ProjectSkills)
                .HasForeignKey(ps => ps.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Certification>(entity =>
        {
            entity.ToTable("ContentCertifications");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(c => c.Issuer)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(c => c.Url)
                .HasMaxLength(2048);
        });
    }
}
