using JaCore.Api.DTOs.Template;
using JaCore.Api.Helpers.Results;

namespace JaCore.Api.Services.Abstractions.Template
{
    public interface ITemplateUIElemService
    {
        Task<Result<IEnumerable<TemplateUIElemDto>>> GetAllTemplateUIElemsAsync();
        Task<Result<TemplateUIElemDto>> GetTemplateUIElemByIdAsync(long id);
        Task<Result<TemplateUIElemDto>> GetTemplateUIElemByNameAsync(string name);
        Task<Result<IEnumerable<TemplateUIElemDto>>> GetTemplateUIElemsByOperationIdAsync(long operationId);
        Task<Result<TemplateUIElemDto>> CreateTemplateUIElemAsync(CreateTemplateUIElemDto createDto);
        Task<Result<TemplateUIElemDto>> UpdateTemplateUIElemAsync(long id, UpdateTemplateUIElemDto updateDto);
        // Delete for TemplateUIElem might be restricted if it's in use by DeviceOperations or TemplateOperations
        Task<Result<bool>> DeleteTemplateUIElemAsync(long id);
        Task<Result<TemplateUIElemDto>> PatchTemplateUIElemAsync(long id, PatchTemplateUIElemDto patchDto);
    }
} 