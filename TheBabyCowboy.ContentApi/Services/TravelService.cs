
using Microsoft.EntityFrameworkCore;
using TheBabyCowboy.ContentApi.Data;
using TheBabyCowBoy.ContentApi.DTOs.Travel;
using TheBabyCowBoy.ContentApi.Models;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

namespace TheBabyCowBoy.ContentApi.Services;

public class TravelService : ITravelService
{
    private readonly ContentDbContext _context;

    public TravelService(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<TravelTripResponseDto>> GetAllAsync()
    {
        var trips = await _context.TravelTrips
            .AsNoTracking()
            .Include(t => t.Photos)
            .OrderBy(t => t.Title)
            .ToListAsync();

        return trips.Select(MapToDto).ToList();
    }

    public async Task<TravelTripResponseDto?> GetByIdAsync(Guid id)
    {
        var trip = await _context.TravelTrips
            .AsNoTracking()
            .Include(t => t.Photos)
            .FirstOrDefaultAsync(t => t.Id == id);

        return trip is null ? null : MapToDto(trip);
    }

    public async Task<TravelTripResponseDto> CreateAsync(
        TravelTripRequestDto request)
    {
        ValidateTrip(request);

        var trip = new TravelTrip
        {
            Title = request.Title.Trim(),
            Location = request.Location.Trim(),
            Country = request.Country.Trim(),
            Description = request.Description.Trim(),
            CoverImageUrl = NormalizeUrl(request.CoverImageUrl)
        };

        _context.TravelTrips.Add(trip);
        await _context.SaveChangesAsync();

        return MapToDto(trip);
    }

    public async Task<TravelTripResponseDto?> UpdateAsync(
        Guid id,
        TravelTripRequestDto request)
    {
        ValidateTrip(request);

        var trip = await _context.TravelTrips
            .Include(t => t.Photos)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trip is null)
            return null;

        trip.Title = request.Title.Trim();
        trip.Location = request.Location.Trim();
        trip.Country = request.Country.Trim();
        trip.Description = request.Description.Trim();
        trip.CoverImageUrl = NormalizeUrl(request.CoverImageUrl);

        await _context.SaveChangesAsync();

        return MapToDto(trip);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var trip = await _context.TravelTrips
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trip is null)
            return false;

        // The configured cascade delete removes its photo records.
        _context.TravelTrips.Remove(trip);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TravelPhotoResponseDto?> AddPhotoAsync(
        Guid tripId,
        TravelPhotoRequestDto request)
    {
        ValidatePhoto(request);

        var trip = await _context.TravelTrips
            .FirstOrDefaultAsync(t => t.Id == tripId);

        if (trip is null)
            return null;

        var photo = new TravelPhoto
        {
            TravelTripId = tripId,
            ImageUrl = request.ImageUrl.Trim(),
            Caption = string.IsNullOrWhiteSpace(request.Caption)
                ? null
                : request.Caption.Trim()
        };

        _context.TravelPhotos.Add(photo);
        await _context.SaveChangesAsync();

        return MapPhotoToDto(photo);
    }

    public async Task<bool> DeletePhotoAsync(
        Guid tripId,
        Guid photoId)
    {
        var photo = await _context.TravelPhotos
            .FirstOrDefaultAsync(p =>
                p.Id == photoId &&
                p.TravelTripId == tripId);

        if (photo is null)
            return false;

        _context.TravelPhotos.Remove(photo);
        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidateTrip(TravelTripRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Trip title is required.");

        if (request.Title.Trim().Length > 200)
            throw new ArgumentException(
                "Trip title cannot exceed 200 characters.");

        if (string.IsNullOrWhiteSpace(request.Location))
            throw new ArgumentException("Location is required.");

        if (request.Location.Trim().Length > 200)
            throw new ArgumentException(
                "Location cannot exceed 200 characters.");

        if (string.IsNullOrWhiteSpace(request.Country))
            throw new ArgumentException("Country is required.");

        if (request.Country.Trim().Length > 100)
            throw new ArgumentException(
                "Country cannot exceed 100 characters.");

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description is required.");

        ValidateOptionalUrl(request.CoverImageUrl, "Cover image URL");
    }

    private static void ValidatePhoto(TravelPhotoRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.ImageUrl))
            throw new ArgumentException("Photo image URL is required.");

        ValidateOptionalUrl(request.ImageUrl, "Photo image URL");

        if (request.Caption?.Trim().Length > 500)
            throw new ArgumentException(
                "Photo caption cannot exceed 500 characters.");
    }

    private static void ValidateOptionalUrl(
        string? value,
        string fieldName)
    {
        var url = NormalizeUrl(value);

        if (url is null)
            return;

        if (url.Length > 2048 ||
            !Uri.TryCreate(url, UriKind.Absolute, out var parsed) ||
            (parsed.Scheme != Uri.UriSchemeHttp &&
             parsed.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException(
                $"{fieldName} must be a valid HTTP or HTTPS URL.");
        }
    }

    private static string? NormalizeUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static TravelTripResponseDto MapToDto(TravelTrip trip)
    {
        return new TravelTripResponseDto
        {
            Id = trip.Id,
            Title = trip.Title,
            Location = trip.Location,
            Country = trip.Country,
            Description = trip.Description,
            CoverImageUrl = trip.CoverImageUrl,
            Photos = trip.Photos
                .OrderBy(p => p.Id)
                .Select(MapPhotoToDto)
                .ToList()
        };
    }

    private static TravelPhotoResponseDto MapPhotoToDto(
        TravelPhoto photo)
    {
        return new TravelPhotoResponseDto
        {
            Id = photo.Id,
            ImageUrl = photo.ImageUrl,
            Caption = photo.Caption
        };
    }
}
