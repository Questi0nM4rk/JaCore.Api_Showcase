using AutoMapper;
using JaCore.Api.DTOs.Template;
using JaCore.Api.Entities.Template;
using JaCore.Api.Helpers.Results;
using JaCore.Api.Repositories.Abstractions;
using JaCore.Api.Services.Abstractions.Template;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using JaCore.Api.Helpers;

namespace JaCore.Api.Services.Template
{
    public class TemplateUIElemService : ITemplateUIElemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TemplateUIElemService> _logger;

        public TemplateUIElemService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TemplateUIElemService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<TemplateUIElemDto>>> GetAllTemplateUIElemsAsync()
        {
            var elems = await _unitOfWork.TemplateUIElems.GetAllAsync();
            return Result<IEnumerable<TemplateUIElemDto>>.Success(_mapper.Map<IEnumerable<TemplateUIElemDto>>(elems));
        }

        public async Task<Result<TemplateUIElemDto>> GetTemplateUIElemByIdAsync(long id)
        {
            var elem = await _unitOfWork.TemplateUIElems.GetByIdAsync(id);
            if (elem == null)
            {
                return Result.Failure<TemplateUIElemDto>(ErrorHelper.NotFound($"TemplateUIElem with ID {id} not found."));
            }
            return Result.Success(_mapper.Map<TemplateUIElemDto>(elem));
        }

        public async Task<Result<TemplateUIElemDto>> GetTemplateUIElemByNameAsync(string name)
        {
            var elem = await _unitOfWork.TemplateUIElems.FirstOrDefaultAsync(e => e.Name == name);
            if (elem == null)
            {
                return Result.Failure<TemplateUIElemDto>(ErrorHelper.NotFound($"TemplateUIElem with name '{name}' not found."));
            }
            return Result.Success(_mapper.Map<TemplateUIElemDto>(elem));
        }

        public async Task<Result<TemplateUIElemDto>> CreateTemplateUIElemAsync(CreateTemplateUIElemDto createDto)
        {
            var existingElem = await _unitOfWork.TemplateUIElems.FirstOrDefaultAsync(e => e.Name == createDto.Name);
            if (existingElem != null)
            {
                return Result.Failure<TemplateUIElemDto>(ErrorHelper.Conflict($"TemplateUIElem with name '{createDto.Name}' already exists."));
            }

            var elem = _mapper.Map<TemplateUIElem>(createDto);
            await _unitOfWork.TemplateUIElems.AddAsync(elem);
            await _unitOfWork.CompleteAsync();

            return Result.Success(_mapper.Map<TemplateUIElemDto>(elem));
        }

        public async Task<Result<TemplateUIElemDto>> UpdateTemplateUIElemAsync(long id, UpdateTemplateUIElemDto updateDto)
        {
            var elem = await _unitOfWork.TemplateUIElems.GetByIdAsync(id);
            if (elem == null)
            {
                return Result.Failure<TemplateUIElemDto>(ErrorHelper.NotFound($"TemplateUIElem with ID {id} not found."));
            }

            if (elem.Name != updateDto.Name)
            {
                var existingElem = await _unitOfWork.TemplateUIElems.FirstOrDefaultAsync(e => e.Name == updateDto.Name && e.Id != id);
                if (existingElem != null)
                {
                    return Result.Failure<TemplateUIElemDto>(ErrorHelper.Conflict($"TemplateUIElem with name '{updateDto.Name}' already exists."));
                }
            }

            _mapper.Map(updateDto, elem);
            _unitOfWork.TemplateUIElems.Update(elem);
            await _unitOfWork.CompleteAsync();

            return Result.Success(_mapper.Map<TemplateUIElemDto>(elem));
        }

        public async Task<Result<bool>> DeleteTemplateUIElemAsync(long id)
        {
            var elem = await _unitOfWork.TemplateUIElems.GetByIdAsync(id);
            if (elem == null)
            {
                return Result.Failure<bool>(ErrorHelper.NotFound($"TemplateUIElem with ID {id} not found."));
            }

            // Check for dependencies in DeviceOperations
            var deviceOperationsCount = await _unitOfWork.DeviceOperations.CountAsync(op => op.TemplateUIElemId == id && !op.IsRemoved);
            if (deviceOperationsCount > 0)
            {
                return Result.Failure<bool>(ErrorHelper.Validation($"TemplateUIElem with ID {id} cannot be deleted because it is used by {deviceOperationsCount} device operation(s)."));
            }

            // Soft delete is not applicable to TemplateUIElem as per entity definition
            // await _unitOfWork.TemplateUIElems.RemoveAsync(id); 
            // For hard delete, if the repository has a generic Remove(entity) or RemoveById(id)
            _unitOfWork.TemplateUIElems.Remove(elem); // Assuming a Remove(T entity) method exists
            await _unitOfWork.CompleteAsync();

            return Result.Success(true);
        }

        public async Task<Result<TemplateUIElemDto>> PatchTemplateUIElemAsync(long id, PatchTemplateUIElemDto patchDto)
        {
            var templateUIElem = await _unitOfWork.TemplateUIElems.GetByIdAsync(id);
            if (templateUIElem == null)
            {
                return Result.Failure<TemplateUIElemDto>(ErrorHelper.NotFound($"TemplateUIElem with ID {id} not found."));
            }

            // Check for duplicate Name if it's being changed
            if (patchDto.Name != null && patchDto.Name != templateUIElem.Name)
            {
                var existingElem = await _unitOfWork.TemplateUIElems.FirstOrDefaultAsync(e => 
                    e.Name == patchDto.Name && 
                    e.Id != id);
                if (existingElem != null)
                {
                    return Result.Failure<TemplateUIElemDto>(ErrorHelper.Conflict($"A TemplateUIElem with the name '{patchDto.Name}' already exists."));
                }
                templateUIElem.Name = patchDto.Name;
            }

            // Apply patchable ElemType
            if (patchDto.ElemType.HasValue && patchDto.ElemType.Value != templateUIElem.ElemType)
            {
                templateUIElem.ElemType = patchDto.ElemType.Value;
            }

            _unitOfWork.TemplateUIElems.Update(templateUIElem);

            try
            {
                await _unitOfWork.CompleteAsync();
                return Result.Success(_mapper.Map<TemplateUIElemDto>(templateUIElem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error patching TemplateUIElem with ID {TemplateUIElemId}", id);
                return Result.Failure<TemplateUIElemDto>(ErrorHelper.ProcessFailure($"An error occurred while patching the TemplateUIElem: {ex.Message}", "PATCH_ERROR"));
            }
        }

        public Task<Result<IEnumerable<TemplateUIElemDto>>> GetTemplateUIElemsByOperationIdAsync(long operationId)
            => throw new System.NotImplementedException();
    }
} 