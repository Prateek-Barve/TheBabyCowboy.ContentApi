
using Microsoft.EntityFrameworkCore;
using TheBabyCowboy.ContentApi.Data;
using TheBabyCowboy.ContentApi.Models;
using TheBabyCowBoy.ContentApi.DTOs.Skills;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Services;

public class SkillService : ISkillService
{
    private readonly ContentDbContext _context;

    public SkillService(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<SkillResponseDto>> GetAllAsync(
        Guid? categoryId = null)
    {
        var query = _context.Skills
            .AsNoTracking()
            .Include(s => s.SkillCategory)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(s =>
                s.SkillCategoryId == categoryId.Value);

        var skills = await query
            .OrderBy(s => s.SkillCategory!.Name)
            .ThenBy(s => s.Name)
            .ToListAsync();

        return skills.Select(MapToDto).ToList();
    }

    public async Task<SkillResponseDto?> GetByIdAsync(Guid id)
    {
        var skill = await _context.Skills
            .AsNoTracking()
            .Include(s => s.SkillCategory)
            .FirstOrDefaultAsync(s => s.Id == id);

        return skill is null ? null : MapToDto(skill);
    }

    public async Task<SkillResponseDto> CreateAsync(
        SkillRequestDto request)
    {
        var name = ValidateName(request.Name);

        var categoryExists = await _context.SkillCategories
            .AnyAsync(c => c.Id == request.SkillCategoryId);

        if (!categoryExists)
            throw new ArgumentException(
                "The specified skill category does not exist.");

        if (await _context.Skills.AnyAsync(s =>
            s.SkillCategoryId == request.SkillCategoryId &&
            s.Name.ToLower() == name.ToLower()))
        {
            throw new ArgumentException(
                "A skill with this name already exists in the category.");
        }

        var skill = new Skill
        {
            Name = name,
            IconUrl = request.IconUrl,
            SkillCategoryId = request.SkillCategoryId
        };

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        await _context.Entry(skill)
            .Reference(s => s.SkillCategory)
            .LoadAsync();

        return MapToDto(skill);
    }

    public async Task<SkillResponseDto?> UpdateAsync(
        Guid id, SkillRequestDto request)
    {
        var skill = await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == id);

        if (skill is null)
            return null;

        var name = ValidateName(request.Name);

        var categoryExists = await _context.SkillCategories
            .AnyAsync(c => c.Id == request.SkillCategoryId);

        if (!categoryExists)
            throw new ArgumentException(
                "The specified skill category does not exist.");

        if (await _context.Skills.AnyAsync(s =>
            s.Id != id &&
            s.SkillCategoryId == request.SkillCategoryId &&
            s.Name.ToLower() == name.ToLower()))
        {
            throw new ArgumentException(
                "A skill with this name already exists in the category.");
        }

        skill.Name = name;
        skill.IconUrl = request.IconUrl;
        skill.SkillCategoryId = request.SkillCategoryId;

        await _context.SaveChangesAsync();

        await _context.Entry(skill)
            .Reference(s => s.SkillCategory)
            .LoadAsync();

        return MapToDto(skill);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var skill = await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == id);

        if (skill is null)
            return false;

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();

        return true;
    }

    private static string ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Skill name is required.");

        name = name.Trim();

        if (name.Length > 100)
            throw new ArgumentException(
                "Skill name cannot exceed 100 characters.");

        return name;
    }

    private static SkillResponseDto MapToDto(Skill skill)
    {
        return new SkillResponseDto
        {
            Id = skill.Id,
            Name = skill.Name,
            IconUrl = skill.IconUrl,
            SkillCategoryId = skill.SkillCategoryId,
            CategoryName = skill.SkillCategory?.Name ?? string.Empty
        };
    }
}
