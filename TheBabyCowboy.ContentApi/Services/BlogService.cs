
using Microsoft.EntityFrameworkCore;
using TheBabyCowboy.ContentApi.Data;
using TheBabyCowBoy.ContentApi.DTOs.Blogs;
using TheBabyCowBoy.ContentApi.Models;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Services;

public class BlogService : IBlogService
{
    private readonly ContentDbContext _context;

    public BlogService(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<BlogResponseDto>> GetAllAsync()
    {
        var blogs = await _context.Blogs
            .AsNoTracking()
            .OrderByDescending(b => b.PublishedAt ?? b.CreatedAt)
            .ToListAsync();

        return blogs.Select(MapToDto).ToList();
    }

    public async Task<BlogResponseDto?> GetByIdAsync(Guid id)
    {
        var blog = await _context.Blogs
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        return blog is null ? null : MapToDto(blog);
    }

    public async Task<BlogResponseDto> CreateAsync(
        BlogRequestDto request)
    {
        Validate(request);

        var now = DateTimeOffset.UtcNow;

        var blog = new Blog
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Category = request.Category.Trim(),
            Tags = NormalizeTags(request.Tags),
            CoverImageUrl = NormalizeOptionalUrl(
                request.CoverImageUrl),
            ExternalUrl = request.ExternalUrl.Trim(),
            PublishedAt = request.PublishedAt ?? now,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Blogs.Add(blog);

        await _context.SaveChangesAsync();

        return MapToDto(blog);
    }

    public async Task<BlogResponseDto?> UpdateAsync(
        Guid id,
        BlogRequestDto request)
    {
        Validate(request);

        var blog = await _context.Blogs
            .FirstOrDefaultAsync(b => b.Id == id);

        if (blog is null)
        {
            return null;
        }

        blog.Title = request.Title.Trim();
        blog.Description = request.Description.Trim();
        blog.Category = request.Category.Trim();
        blog.Tags = NormalizeTags(request.Tags);
        blog.CoverImageUrl = NormalizeOptionalUrl(
            request.CoverImageUrl);
        blog.ExternalUrl = request.ExternalUrl.Trim();

        // Preserve the existing publication date when omitted.
        if (request.PublishedAt.HasValue)
        {
            blog.PublishedAt = request.PublishedAt.Value;
        }

        blog.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(blog);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var blog = await _context.Blogs
            .FirstOrDefaultAsync(b => b.Id == id);

        if (blog is null)
        {
            return false;
        }

        _context.Blogs.Remove(blog);

        await _context.SaveChangesAsync();

        return true;
    }

    private static void Validate(BlogRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                "Title is required.");
        }

        if (request.Title.Trim().Length > 250)
        {
            throw new ArgumentException(
                "Title cannot exceed 250 characters.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new ArgumentException(
                "Description is required.");
        }

        if (request.Description.Trim().Length > 500)
        {
            throw new ArgumentException(
                "Description cannot exceed 500 characters.");
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            throw new ArgumentException(
                "Category is required.");
        }

        if (request.Category.Trim().Length > 100)
        {
            throw new ArgumentException(
                "Category cannot exceed 100 characters.");
        }

        if (request.Tags is null)
        {
            throw new ArgumentException(
                "Tags cannot be null.");
        }

        if (request.Tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(
                "Tags cannot contain empty values.");
        }

        if (request.Tags.Any(tag => tag.Trim().Length > 100))
        {
            throw new ArgumentException(
                "Each tag cannot exceed 100 characters.");
        }

        ValidateHttpUrl(request.ExternalUrl, "ExternalUrl");

        if (!string.IsNullOrWhiteSpace(request.CoverImageUrl))
        {
            ValidateHttpUrl(
                request.CoverImageUrl,
                "CoverImageUrl");
        }
    }

    private static void ValidateHttpUrl(
        string? value,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Trim().Length > 2048 ||
            !Uri.TryCreate(
                value.Trim(),
                UriKind.Absolute,
                out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp &&
             uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException(
                $"{fieldName} must be a valid HTTP or HTTPS URL.");
        }
    }

    private static string? NormalizeOptionalUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static List<string> NormalizeTags(
        IEnumerable<string> tags)
    {
        return tags
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static BlogResponseDto MapToDto(Blog blog)
    {
        return new BlogResponseDto
        {
            Id = blog.Id,
            Title = blog.Title,
            Description = blog.Description,
            Category = blog.Category,
            Tags = blog.Tags,
            CoverImageUrl = blog.CoverImageUrl,
            ExternalUrl = blog.ExternalUrl,
            PublishedAt = blog.PublishedAt,
            CreatedAt = blog.CreatedAt,
            UpdatedAt = blog.UpdatedAt
        };
    }
}
