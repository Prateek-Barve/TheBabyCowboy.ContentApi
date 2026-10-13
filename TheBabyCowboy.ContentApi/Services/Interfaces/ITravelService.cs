
using TheBabyCowBoy.ContentApi.DTOs.Travel;

namespace TheBabyCowBoy.ContentApi.Services.Interfaces;

public interface ITravelService
{
    Task<List<TravelTripResponseDto>> GetAllAsync();

    Task<TravelTripResponseDto?> GetByIdAsync(Guid id);

    Task<TravelTripResponseDto> CreateAsync(
        TravelTripRequestDto request);

    Task<TravelTripResponseDto?> UpdateAsync(
        Guid id,
        TravelTripRequestDto request);

    Task<bool> DeleteAsync(Guid id);

    Task<TravelPhotoResponseDto?> AddPhotoAsync(
        Guid tripId,
        TravelPhotoRequestDto request);

    Task<bool> DeletePhotoAsync(
        Guid tripId,
        Guid photoId);
}
