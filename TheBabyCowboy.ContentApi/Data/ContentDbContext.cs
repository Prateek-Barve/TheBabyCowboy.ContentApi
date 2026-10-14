
using Microsoft.EntityFrameworkCore;
using TheBabyCowboy.ContentApi.Models;
using TheBabyCowBoy.ContentApi.Models;

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
    public DbSet<TravelTrip> TravelTrips => Set<TravelTrip>();

    public DbSet<TravelPhoto> TravelPhotos => Set<TravelPhoto>();
    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();

    public DbSet<JournalImage> JournalImages => Set<JournalImage>();

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


        modelBuilder.Entity<TravelTrip>(entity =>
        {
            entity.ToTable("TravelTrips");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(t => t.Location)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(t => t.Country)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(t => t.Description)
                .IsRequired();

            entity.Property(t => t.CoverImageUrl)
                .HasMaxLength(2048);
        });

        modelBuilder.Entity<TravelPhoto>(entity =>
        {
            entity.ToTable("TravelPhotos");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.ImageUrl)
                .HasMaxLength(2048)
                .IsRequired();

            entity.Property(p => p.Caption)
                .HasMaxLength(500);

            entity.HasOne(p => p.TravelTrip)
                .WithMany(t => t.Photos)
                .HasForeignKey(p => p.TravelTripId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.ToTable("ContentBlogs");

            entity.HasKey(b => b.Id);

            entity.Property(b => b.Title)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(b => b.Excerpt)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(b => b.Description)
                .IsRequired();

            entity.Property(b => b.Category)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(b => b.Tags)
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(b => b.CoverImageUrl)
                .HasMaxLength(2048);

            entity.Property(b => b.PublishedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(b => b.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(b => b.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();
            entity.Property(b => b.ExternalUrl)
    .HasMaxLength(2048)
    .IsRequired();

            entity.HasIndex(b => b.PublishedAt);
        });

        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.ToTable("ContentJournalEntries");

            entity.HasKey(j => j.Id);

            entity.Property(j => j.Title)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(j => j.Content)
                .IsRequired();

            entity.Property(j => j.Mood)
                .HasMaxLength(50);

            entity.Property(j => j.Category)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(j => j.Tags)
                .HasColumnType("text[]")
                .IsRequired();

            entity.Property(j => j.IsPublic)
                .HasDefaultValue(false);

            entity.HasIndex(j => j.CreatedAt);

            entity.HasIndex(j => new { j.IsPublic, j.CreatedAt });
        });

        modelBuilder.Entity<JournalImage>(entity =>
        {
            entity.ToTable("ContentJournalImages");

            entity.HasKey(i => i.Id);

            entity.Property(i => i.ImageUrl)
                .HasMaxLength(2048)
                .IsRequired();

            entity.Property(i => i.Caption)
                .HasMaxLength(500);

            entity.HasOne(i => i.JournalEntry)
                .WithMany(j => j.Images)
                .HasForeignKey(i => i.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(i => i.JournalEntryId);
        });
    }
}
