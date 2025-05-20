using AutoMapper;
using JaCore.Api.DTOs.Common;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Entities.Device;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions;
using JaCore.Api.Services.Abstractions.Device;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using JaCore.Api.Helpers;
using System.Collections.Generic;

namespace JaCore.Api.Services.Device
{
    public class MetConfirmationService : IMetConfirmationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MetConfirmationService> _logger;

        public MetConfirmationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MetConfirmationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedResult<MetConfirmationDto>>> GetAllMetConfirmationsAsync(QueryParametersDto queryParameters)
        {
            var pagedResult = await _unitOfWork.MetConfirmations.GetAllAsync(queryParameters);
            var metConfirmationDtos = _mapper.Map<List<MetConfirmationDto>>(pagedResult.Items);
            var pagedDtoResult = new PagedResult<MetConfirmationDto>(
                metConfirmationDtos,
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize
            );
            return Result.Success(pagedDtoResult);
        }

        public async Task<Result<MetConfirmationDto>> GetMetConfirmationByIdAsync(long id, string? include = null)
        {
            var item = await _unitOfWork.MetConfirmations.GetByIdAsync(id, includeProperties: include);
            if (item == null)
            {
                return Result.Failure<MetConfirmationDto>(ErrorHelper.NotFound($"MetConfirmation with ID {id} not found."));
            }
            return Result.Success(_mapper.Map<MetConfirmationDto>(item));
        }

        public async Task<Result<MetConfirmationDto>> CreateMetConfirmationAsync(CreateMetConfirmationDto createDto)
        {
            if (await _unitOfWork.MetConfirmations.ExistsAsync(mc => mc.Name == createDto.Name && !mc.IsRemoved))
            {
                return Result.Failure<MetConfirmationDto>(ErrorHelper.Conflict($"A MetConfirmation with the name '{createDto.Name}' already exists."));
            }

            var newItem = _mapper.Map<MetConfirmation>(createDto);
            await _unitOfWork.MetConfirmations.AddAsync(newItem);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<MetConfirmationDto>(newItem));
        }

        public async Task<Result<MetConfirmationDto>> UpdateMetConfirmationAsync(long id, UpdateMetConfirmationDto updateDto)
        {
            var item = await _unitOfWork.MetConfirmations.GetByIdAsync(id);
            if (item == null)
            {
                return Result.Failure<MetConfirmationDto>(ErrorHelper.NotFound($"MetConfirmation with ID {id} not found to update."));
            }

            if (item.Name != updateDto.Name &&
                await _unitOfWork.MetConfirmations.ExistsAsync(mc => mc.Name == updateDto.Name && mc.Id != id && !mc.IsRemoved))
            {
                return Result.Failure<MetConfirmationDto>(ErrorHelper.Conflict($"Another MetConfirmation with the name '{updateDto.Name}' already exists."));
            }

            _mapper.Map(updateDto, item);
            _unitOfWork.MetConfirmations.Update(item);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<MetConfirmationDto>(item));
        }

        public async Task<Result<bool>> DeleteMetConfirmationAsync(long id)
        {
            var item = await _unitOfWork.MetConfirmations.GetByIdAsync(id);
            if (item == null)
            {
                return Result.Failure<bool>(ErrorHelper.NotFound("MetConfirmation", id.ToString()));
            }

            // Check for dependencies: if DeviceCards are using this MetConfirmation
            if (await _unitOfWork.DeviceCards.ExistsAsync(dc => dc.MetConfirmationId == id && !dc.IsRemoved))
            {
                return Result.Failure<bool>(ErrorHelper.Validation($"MetConfirmation with ID {id} is currently in use and cannot be deleted."));
            }

            _unitOfWork.MetConfirmations.Remove(item);
            await _unitOfWork.CompleteAsync();
            return Result.Success(true);
        }

        public async Task<Result<MetConfirmationDto>> PatchMetConfirmationAsync(long id, PatchMetConfirmationDto patchDto)
        {
            var metConf = await _unitOfWork.MetConfirmations.GetByIdAsync(id);
            if (metConf == null || metConf.IsRemoved)
            {
                return Result.Failure<MetConfirmationDto>(ErrorHelper.NotFound("MetConfirmation", id.ToString()));
            }

            if (patchDto.Name != null)
            {
                if (metConf.Name != patchDto.Name && !await _unitOfWork.MetConfirmations.IsNameUniqueAsync(patchDto.Name, metConf.Id))
                {
                    return Result.Failure<MetConfirmationDto>(ErrorHelper.Conflict("MetConfirmation name already exists."));
                }
                metConf.Name = patchDto.Name;
            }
            if (patchDto.Lvl1 != null) metConf.Lvl1 = patchDto.Lvl1;
            if (patchDto.Lvl2 != null) metConf.Lvl2 = patchDto.Lvl2;
            if (patchDto.Lvl3 != null) metConf.Lvl3 = patchDto.Lvl3;
            if (patchDto.Lvl4 != null) metConf.Lvl4 = patchDto.Lvl4;

            _unitOfWork.MetConfirmations.Update(metConf);

            try
            {
                await _unitOfWork.CompleteAsync();
                return Result.Success(_mapper.Map<MetConfirmationDto>(metConf));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error patching MetConfirmation with ID {MetConfirmationId}", id);
                return Result.Failure<MetConfirmationDto>(ErrorHelper.ProcessFailure($"An error occurred while patching the MetConfirmation: {ex.Message}", "PATCH_ERROR"));
            }
        }
    }
} 