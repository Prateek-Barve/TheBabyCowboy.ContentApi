
using Microsoft.EntityFrameworkCore;
using TheBabyCowboy.ContentApi.Data;
using TheBabyCowBoy.ContentApi.DTOs.Journals;
using TheBabyCowBoy.ContentApi.Models;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Services;

public class JournalService : IJournalService
{
    private readonly ContentDbContext _context;

    public JournalService(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<JournalResponseDto>> GetAllAsync(
        bool includePrivate,
        string? search,
        string? mood,
        string? category,
        string? tag,
        DateTimeOffset? from,
        DateTimeOffset? to)
    {
        ValidateDateRange(from, to);

        var query = _context.JournalEntries
            .AsNoTracking()
            .Include(j => j.Images)
            .AsQueryable();

        if (!includePrivate)
            query = query.Where(j => j.IsPublic);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(j =>
                EF.Functions.ILike(j.Title, $"%{term}%") ||
                EF.Functions.ILike(j.Content, $"%{term}%"));
        }

        if (!string.IsNullOrWhiteSpace(mood))
        {
            var selectedMood = mood.Trim();

            query = query.Where(j => j.Mood == selectedMood);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var selectedCategory = category.Trim();

            query = query.Where(j => j.Category == selectedCategory);
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var selectedTag = tag.Trim();

            query = query.Where(j => j.Tags.Contains(selectedTag));
        }

        if (from.HasValue)
            query = query.Where(j => j.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(j => j.CreatedAt <= to.Value);

        var entries = await query
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return entries.Select(MapToDto).ToList();
    }

    public async Task<JournalResponseDto?> GetByIdAsync(
        Guid id,
        bool includePrivate)
    {
        var query = _context.JournalEntries
            .AsNoTracking()
            .Include(j => j.Images)
            .Where(j => j.Id == id);

        if (!includePrivate)
            query = query.Where(j => j.IsPublic);

        var entry = await query.FirstOrDefaultAsync();

        return entry is null ? null : MapToDto(entry);
    }

    public async Task<JournalResponseDto> CreateAsync(
        JournalRequestDto request)
    {
        ValidateRequest(request);

        var now = DateTimeOffset.UtcNow;

        var entry = new JournalEntry
        {
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            Mood = NormalizeOptional(request.Mood),
            Category = request.Category.Trim(),
            Tags = NormalizeTags(request.Tags),
            IsPublic = request.IsPublic,
            CreatedAt = now,
            UpdatedAt = now,
            Images = request.Images.Select(image =>
                new JournalImage
                {
                    ImageUrl = image.ImageUrl.Trim(),
                    Caption = NormalizeOptional(image.Caption),
                    SortOrder = image.SortOrder
                }).ToList()
        };

        _context.JournalEntries.Add(entry);

        await _context.SaveChangesAsync();

        return MapToDto(entry);
    }

    //public async Task<JournalResponseDto?> UpdateAsync(
    //    Guid id,
    //    JournalRequestDto request)
    //{
    //    ValidateRequest(request);

    //    var entry = await _context.JournalEntries
    //        .Include(j => j.Images)
    //        .FirstOrDefaultAsync(j => j.Id == id);

    //    if (entry is null)
    //        return null;

    //    entry.Title = request.Title.Trim();
    //    entry.Content = request.Content.Trim();
    //    entry.Mood = NormalizeOptional(request.Mood);
    //    entry.Category = request.Category.Trim();
    //    entry.Tags = NormalizeTags(request.Tags);
    //    entry.IsPublic = request.IsPublic;
    //    entry.UpdatedAt = DateTimeOffset.UtcNow;

    //    // Replace the entry's image collection with the submitted list.
    //    _context.JournalImages.RemoveRange(entry.Images);
    //    entry.Images.Clear();

    //    foreach (var image in request.Images)
    //    {
    //        entry.Images.Add(new JournalImage
    //        {
    //            Id = Guid.NewGuid(),
    //            ImageUrl = image.ImageUrl.Trim(),
    //            Caption = NormalizeOptional(image.Caption),
    //            SortOrder = image.SortOrder
    //        });
    //    }

    //    await _context.SaveChangesAsync();

    //    return MapToDto(entry);
    //}

    public async Task<JournalResponseDto?> UpdateAsync(
        Guid id,
        JournalRequestDto request)
    {
        ValidateRequest(request);

        var entry = await _context.JournalEntries
            .FirstOrDefaultAsync(j => j.Id == id);

        if (entry is null)
            return null;

        entry.Title = request.Title.Trim();
        entry.Content = request.Content.Trim();
        entry.Mood = NormalizeOptional(request.Mood);
        entry.Category = request.Category.Trim();
        entry.Tags = NormalizeTags(request.Tags);
        entry.IsPublic = request.IsPublic;
        entry.UpdatedAt = DateTimeOffset.UtcNow;

        // Load existing images separately so the navigation collection
        // is not simultaneously tracked and cleared.
        var existingImages = await _context.JournalImages
            .Where(i => i.JournalEntryId == id)
            .ToListAsync();

        _context.JournalImages.RemoveRange(existingImages);

        // Add the replacement images directly through the DbSet.
        var newImages = request.Images.Select(image =>
            new JournalImage
            {
                Id = Guid.NewGuid(),
                JournalEntryId = entry.Id,
                ImageUrl = image.ImageUrl.Trim(),
                Caption = NormalizeOptional(image.Caption),
                SortOrder = image.SortOrder
            }).ToList();

        _context.JournalImages.AddRange(newImages);

        await _context.SaveChangesAsync();

        // Reload the entry with its updated images for the response.
        await _context.Entry(entry)
            .Collection(j => j.Images)
            .LoadAsync();

        return MapToDto(entry);
    }


    public async Task<bool> DeleteAsync(Guid id)
    {
        var entry = await _context.JournalEntries
            .FirstOrDefaultAsync(j => j.Id == id);

        if (entry is null)
            return false;

        _context.JournalEntries.Remove(entry);

        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidateRequest(JournalRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title is required.");

        if (request.Title.Trim().Length > 250)
            throw new ArgumentException(
                "Title cannot exceed 250 characters.");

        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ArgumentException("Content is required.");

        if (string.IsNullOrWhiteSpace(request.Category))
            throw new ArgumentException("Category is required.");

        if (request.Category.Trim().Length > 100)
            throw new ArgumentException(
                "Category cannot exceed 100 characters.");

        if (request.Mood?.Trim().Length > 50)
            throw new ArgumentException(
                "Mood cannot exceed 50 characters.");

        if (request.Tags is null)
            throw new ArgumentException("Tags cannot be null.");

        if (request.Tags.Any(t =>
            string.IsNullOrWhiteSpace(t) || t.Trim().Length > 100))
        {
            throw new ArgumentException(
                "Tags must be non-empty and cannot exceed 100 characters.");
        }

        if (request.Images is null)
            throw new ArgumentException("Images cannot be null.");

        foreach (var image in request.Images)
        {
            if (!Uri.TryCreate(
                image.ImageUrl,
                UriKind.Absolute,
                out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttps &&
                 uri.Scheme != Uri.UriSchemeHttp))
            {
                throw new ArgumentException(
                    "Each image must have a valid HTTP or HTTPS URL.");
            }

            if (image.ImageUrl.Length > 2048)
                throw new ArgumentException(
                    "Image URLs cannot exceed 2048 characters.");

            if (image.Caption?.Length > 500)
                throw new ArgumentException(
                    "Image captions cannot exceed 500 characters.");
        }
    }

    private static void ValidateDateRange(
        DateTimeOffset? from,
        DateTimeOffset? to)
    {
        if (from.HasValue && to.HasValue && from.Value > to.Value)
        {
            throw new ArgumentException(
                "'from' must be earlier than or equal to 'to'.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static List<string> NormalizeTags(List<string> tags)
    {
        return tags
            .Select(t => t.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static JournalResponseDto MapToDto(JournalEntry entry)
    {
        return new JournalResponseDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Content = entry.Content,
            Mood = entry.Mood,
            Category = entry.Category,
            Tags = entry.Tags.ToList(),
            IsPublic = entry.IsPublic,
            CreatedAt = entry.CreatedAt,
            UpdatedAt = entry.UpdatedAt,
            Images = entry.Images
                .OrderBy(i => i.SortOrder)
                .Select(i => new JournalImageResponseDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    SortOrder = i.SortOrder
                })
                .ToList()
        };
    }
}
