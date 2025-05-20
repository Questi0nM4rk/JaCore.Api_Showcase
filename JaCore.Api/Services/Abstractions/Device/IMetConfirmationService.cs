using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Helpers.Results;

namespace JaCore.Api.Services.Abstractions.Device
{
    public interface IMetConfirmationService
    {
        Task<Result<PagedResult<MetConfirmationDto>>> GetAllMetConfirmationsAsync(QueryParametersDto queryParameters);
        Task<Result<MetConfirmationDto>> GetMetConfirmationByIdAsync(long id, string? include = null);
        Task<Result<MetConfirmationDto>> CreateMetConfirmationAsync(CreateMetConfirmationDto createDto);
        Task<Result<MetConfirmationDto>> UpdateMetConfirmationAsync(long id, UpdateMetConfirmationDto updateDto);
        Task<Result<bool>> DeleteMetConfirmationAsync(long id);
        Task<Result<MetConfirmationDto>> PatchMetConfirmationAsync(long id, PatchMetConfirmationDto patchDto);
    }
} 