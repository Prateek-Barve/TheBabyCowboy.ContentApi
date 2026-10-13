
using Microsoft.EntityFrameworkCore;
using TheBabyCowboy.ContentApi.Data;
using TheBabyCowboy.ContentApi.Models;
using TheBabyCowBoy.ContentApi.DTOs.Projects;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Services;

public class ProjectService : IProjectService
{
    private readonly ContentDbContext _context;

    public ProjectService(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProjectResponseDto>> GetAllAsync()
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
                    .ThenInclude(s => s.SkillCategory)
            .OrderBy(p => p.Title)
            .ToListAsync();

        return projects.Select(MapToDto).ToList();
    }

    public async Task<ProjectResponseDto?> GetByIdAsync(Guid id)
    {
        var project = await _context.Projects
            .AsNoTracking()
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
                    .ThenInclude(s => s.SkillCategory)
            .FirstOrDefaultAsync(p => p.Id == id);

        return project is null ? null : MapToDto(project);
    }

    public async Task<ProjectResponseDto> CreateAsync(
        ProjectRequestDto request)
    {
        ValidateStatus(request.Status);

        var skills = await GetSkillsAsync(request.SkillIds);

        var project = new Project
        {
            Title = request.Title.Trim(),
            ShortDescription = request.ShortDescription.Trim(),
            Description = request.Description.Trim(),
            Status = request.Status,
            ThumbnailUrl = request.ThumbnailUrl,
            GitHubUrl = request.GitHubUrl
        };

        foreach (var skill in skills)
        {
            project.ProjectSkills.Add(new ProjectSkill
            {
                Project = project,
                Skill = skill
            });
        }

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var createdProject = await _context.Projects
            .AsNoTracking()
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
                    .ThenInclude(s => s.SkillCategory)
            .FirstAsync(p => p.Id == project.Id);

        return MapToDto(createdProject);
    }

    public async Task<ProjectResponseDto?> UpdateAsync(
        Guid id,
        ProjectRequestDto request)
    {
        ValidateStatus(request.Status);

        var project = await _context.Projects
            .Include(p => p.ProjectSkills)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
            return null;

        var skills = await GetSkillsAsync(request.SkillIds);

        project.Title = request.Title.Trim();
        project.ShortDescription = request.ShortDescription.Trim();
        project.Description = request.Description.Trim();
        project.Status = request.Status;
        project.ThumbnailUrl = request.ThumbnailUrl;
        project.GitHubUrl = request.GitHubUrl;

        project.ProjectSkills.Clear();

        foreach (var skill in skills)
        {
            project.ProjectSkills.Add(new ProjectSkill
            {
                ProjectId = project.Id,
                SkillId = skill.Id
            });
        }

        await _context.SaveChangesAsync();

        var updatedProject = await _context.Projects
            .AsNoTracking()
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
                    .ThenInclude(s => s.SkillCategory)
            .FirstAsync(p => p.Id == id);

        return MapToDto(updatedProject);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
            return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task<List<Skill>> GetSkillsAsync(
        IEnumerable<Guid>? requestedIds)
    {
        var skillIds = (requestedIds ?? Enumerable.Empty<Guid>())
            .Distinct()
            .ToList();

        var skills = await _context.Skills
            .Where(s => skillIds.Contains(s.Id))
            .ToListAsync();

        if (skills.Count != skillIds.Count)
        {
            throw new ArgumentException(
                "One or more skill IDs do not exist.");
        }

        return skills;
    }

    private static void ValidateStatus(string status)
    {
        if (status is not ("InProgress" or "Completed" or "Archived"))
        {
            throw new ArgumentException(
                "Status must be InProgress, Completed, or Archived.");
        }
    }

    private static ProjectResponseDto MapToDto(Project project)
    {
        return new ProjectResponseDto
        {
            Id = project.Id,
            Title = project.Title,
            ShortDescription = project.ShortDescription,
            Description = project.Description,
            Status = project.Status,
            ThumbnailUrl = project.ThumbnailUrl,
            GitHubUrl = project.GitHubUrl,
            Skills = project.ProjectSkills
                .Where(ps => ps.Skill is not null)
                .Select(ps => new ProjectSkillDto
                {
                    Id = ps.Skill.Id,
                    Name = ps.Skill.Name,
                    IconUrl = ps.Skill.IconUrl,
                    CategoryName = ps.Skill.SkillCategory?.Name ?? string.Empty
                })
                .ToList()
        };
    }
}
