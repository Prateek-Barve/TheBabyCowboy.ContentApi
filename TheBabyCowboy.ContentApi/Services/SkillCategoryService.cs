
using Microsoft.EntityFrameworkCore;
using TheBabyCowboy.ContentApi.Data;
using TheBabyCowboy.ContentApi.Models;
using TheBabyCowBoy.ContentApi.DTOs.SkillCategories;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Services;

public class SkillCategoryService : ISkillCategoryService
{
    private readonly ContentDbContext _context;

    public SkillCategoryService(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<SkillCategoryResponseDto>> GetAllAsync()
    {
        return await _context.SkillCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new SkillCategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<SkillCategoryResponseDto?> GetByIdAsync(Guid id)
    {
        var category = await _context.SkillCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return category is null ? null : MapToDto(category);
    }

    public async Task<SkillCategoryResponseDto> CreateAsync(
        SkillCategoryRequestDto request)
    {
        var name = ValidateName(request.Name);

        if (await _context.SkillCategories.AnyAsync(
            c => c.Name.ToLower() == name.ToLower()))
        {
            throw new ArgumentException(
                "A skill category with this name already exists.");
        }

        var category = new SkillCategory { Name = name };

        _context.SkillCategories.Add(category);
        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<SkillCategoryResponseDto?> UpdateAsync(
        Guid id, SkillCategoryRequestDto request)
    {
        var category = await _context.SkillCategories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return null;

        var name = ValidateName(request.Name);

        if (await _context.SkillCategories.AnyAsync(
            c => c.Id != id &&
                 c.Name.ToLower() == name.ToLower()))
        {
            throw new ArgumentException(
                "A skill category with this name already exists.");
        }

        category.Name = name;

        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _context.SkillCategories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return false;

        if (await _context.Skills.AnyAsync(s => s.SkillCategoryId == id))
        {
            throw new ArgumentException(
                "This category contains skills. Move or delete those skills before deleting the category.");
        }

        _context.SkillCategories.Remove(category);
        await _context.SaveChangesAsync();

        return true;
    }

    private static string ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.");

        name = name.Trim();

        if (name.Length > 100)
            throw new ArgumentException(
                "Category name cannot exceed 100 characters.");

        return name;
    }

    private static SkillCategoryResponseDto MapToDto(
        SkillCategory category)
    {
        return new SkillCategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}
