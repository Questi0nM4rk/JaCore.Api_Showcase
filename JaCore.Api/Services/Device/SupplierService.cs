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

namespace JaCore.Api.Services.Device
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SupplierService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedResult<SupplierDto>>> GetAllSuppliersAsync(QueryParametersDto queryParameters, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching all suppliers. Query: {@QueryParameters}", queryParameters);
            var pagedResult = await _unitOfWork.Suppliers.GetAllAsync(queryParameters, includeProperties);
            var supplierDtos = _mapper.Map<System.Collections.Generic.List<SupplierDto>>(pagedResult.Items);
            var pagedDtoResult = new PagedResult<SupplierDto>(supplierDtos, pagedResult.PageNumber, pagedResult.PageSize, pagedResult.TotalCount);
            return Result.Success(pagedDtoResult);
        }

        public async Task<Result<SupplierDto>> GetSupplierByIdAsync(long id, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching supplier by ID: {SupplierId}, Include: {Include}", id, includeProperties);
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id, includeProperties);
            if (supplier == null) // Repository GetByIdAsync checks IsRemoved
            {
                return Result.Failure<SupplierDto>(ErrorHelper.NotFound($"Supplier with ID '{id}' not found."));
            }
            return Result.Success(_mapper.Map<SupplierDto>(supplier));
        }

        public async Task<Result<SupplierDto>> GetSupplierByNameAsync(string name, string? includeProperties = null)
        {
            _logger.LogInformation("Fetching supplier by Name: {Name}, Include: {Include}", name, includeProperties);
            var supplier = await _unitOfWork.Suppliers.GetSupplierByNameAsync(name, includeProperties);
            if (supplier == null)
            {
                return Result.Failure<SupplierDto>(ErrorHelper.NotFound($"Supplier with Name '{name}' not found."));
            }
            return Result.Success(_mapper.Map<SupplierDto>(supplier));
        }

        public async Task<Result<SupplierDto>> CreateSupplierAsync(CreateSupplierDto createSupplierDto)
        {
            _logger.LogInformation("Creating new supplier: {@CreateSupplierDto}", createSupplierDto);
            if (!await _unitOfWork.Suppliers.IsNameUniqueAsync(createSupplierDto.Name))
            {
                return Result.Failure<SupplierDto>(ErrorHelper.Conflict($"Supplier with Name '{createSupplierDto.Name}' already exists."));
            }

            var supplier = _mapper.Map<Supplier>(createSupplierDto);
            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<SupplierDto>(supplier));
        }

        public async Task<Result<SupplierDto>> UpdateSupplierAsync(long id, UpdateSupplierDto updateSupplierDto)
        {
            _logger.LogInformation("Updating supplier ID: {SupplierId} with DTO: {@UpdateSupplierDto}", id, updateSupplierDto);
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
            {
                return Result.Failure<SupplierDto>(ErrorHelper.NotFound($"Supplier with ID '{id}' not found."));
            }

            if (supplier.Name != updateSupplierDto.Name && !await _unitOfWork.Suppliers.IsNameUniqueAsync(updateSupplierDto.Name, id))
            {
                return Result.Failure<SupplierDto>(ErrorHelper.Conflict($"Supplier with Name '{updateSupplierDto.Name}' already exists."));
            }

            _mapper.Map(updateSupplierDto, supplier);
            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.CompleteAsync();
            return Result.Success(_mapper.Map<SupplierDto>(supplier));
        }

        public async Task<Result<SupplierDto>> PatchSupplierAsync(long id, PatchSupplierDto patchSupplierDto)
        {
            _logger.LogInformation("Patching supplier ID: {SupplierId} with DTO: {@PatchSupplierDto}", id, patchSupplierDto);
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
            {
                return Result.Failure<SupplierDto>(ErrorHelper.NotFound($"Supplier with ID '{id}' not found."));
            }

            string? originalName = supplier.Name;
            _mapper.Map(patchSupplierDto, supplier);

            if (patchSupplierDto.Name != null && originalName != supplier.Name) // Check if name was patched and changed
            {
                if (!await _unitOfWork.Suppliers.IsNameUniqueAsync(supplier.Name, id))
                {
                     return Result.Failure<SupplierDto>(ErrorHelper.Conflict($"Supplier with Name '{supplier.Name}' already exists."));
                }
            }
            
            try
            {
                _unitOfWork.Suppliers.Update(supplier); // Moved update here
                await _unitOfWork.CompleteAsync();
                return Result.Success(_mapper.Map<SupplierDto>(supplier));
            }
            catch (System.Exception ex) 
            {
                _logger.LogError(ex, "Error patching supplier ID: {SupplierId}", id);
                return Result.Failure<SupplierDto>(ErrorHelper.ProcessFailure($"An error occurred while patching the supplier: {ex.Message}", "PATCH_ERROR")); // Added catch block and ProcessFailure
            }
        }

        public async Task<Result<bool>> DeleteSupplierAsync(long id)
        {
            _logger.LogInformation("Attempting to delete supplier ID: {SupplierId}", id);
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
            {
                return Result.Failure<bool>(ErrorHelper.NotFound($"Supplier with ID '{id}' not found."));
            }

            // Check if any DeviceCard is using this supplier
            var isUsedByDeviceCard = await _unitOfWork.DeviceCards.ExistsAsync(dc => dc.SupplierId == id && !dc.IsRemoved);
            if (isUsedByDeviceCard)
            {
                return Result.Failure<bool>(ErrorHelper.Validation($"Supplier with ID '{id}' is currently in use by one or more device cards and cannot be deleted."));
            }

            _unitOfWork.Suppliers.Remove(supplier); // Handles soft delete
            await _unitOfWork.CompleteAsync();
            return Result.Success(true);
        }
    }
} 