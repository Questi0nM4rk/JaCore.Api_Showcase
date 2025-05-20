using JaCore.Api.DTOs.Template;
using JaCore.Api.Helpers;
using JaCore.Api.Services.Abstractions.Template;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using JaCore.Api.Helpers.Results;
using ResponseError = JaCore.Api.Helpers.ApiError;

namespace JaCore.Api.Controllers.Template
{
    [Route(ApiConstants.BasePaths.TemplateUIElem)]
    public class TemplateUIElemsController : ControllerBase
    {
        private readonly ITemplateUIElemService _templateUIElemService;
        private readonly ILogger<TemplateUIElemsController> _logger;

        public TemplateUIElemsController(ITemplateUIElemService templateUIElemService, ILogger<TemplateUIElemsController> logger)
        {
            _templateUIElemService = templateUIElemService;
            _logger = logger;
        }

        [HttpGet(Name = nameof(GetAllTemplateUIElems))]
        [Authorize(Policy = ApiConstants.Policies.UserAccess)]
        [ProducesResponseType(typeof(IEnumerable<TemplateUIElemDto>), 200)]
        public async Task<IActionResult> GetAllTemplateUIElems()
        {
            var result = await _templateUIElemService.GetAllTemplateUIElemsAsync();
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.TemplateUIElemEndpoints.ById, Name = nameof(GetTemplateUIElemById))]
        [Authorize(Policy = ApiConstants.Policies.UserAccess)]
        [ProducesResponseType(typeof(TemplateUIElemDto), 200)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        public async Task<IActionResult> GetTemplateUIElemById(long id)
        {
            var result = await _templateUIElemService.GetTemplateUIElemByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpGet(ApiConstants.TemplateUIElemEndpoints.ByName, Name = nameof(GetTemplateUIElemByName))]
        [Authorize(Policy = ApiConstants.Policies.UserAccess)]
        [ProducesResponseType(typeof(TemplateUIElemDto), 200)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        public async Task<IActionResult> GetTemplateUIElemByName(string name)
        {
            var result = await _templateUIElemService.GetTemplateUIElemByNameAsync(name);
            return result.ToActionResult(this);
        }

        [HttpPost(Name = nameof(CreateTemplateUIElem))]
        [Authorize(Policy = ApiConstants.Policies.ManagementAccess)]
        [ProducesResponseType(typeof(TemplateUIElemDto), 201)]
        [ProducesResponseType(typeof(ResponseError), 400)]
        [ProducesResponseType(typeof(ResponseError), 409)]
        public async Task<IActionResult> CreateTemplateUIElem([FromBody] CreateTemplateUIElemDto createDto)
        {
            var result = await _templateUIElemService.CreateTemplateUIElemAsync(createDto);
            return result.ToActionResult(this, nameof(GetTemplateUIElemById), new { id = result.Value?.Id });
        }

        [HttpPut(ApiConstants.TemplateUIElemEndpoints.ById, Name = nameof(UpdateTemplateUIElem))]
        [Authorize(Policy = ApiConstants.Policies.ManagementAccess)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ResponseError), 400)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        [ProducesResponseType(typeof(ResponseError), 409)]
        public async Task<IActionResult> UpdateTemplateUIElem(long id, [FromBody] UpdateTemplateUIElemDto updateDto)
        {
            var result = await _templateUIElemService.UpdateTemplateUIElemAsync(id, updateDto);
            return result.ToActionResult(this);
        }

        // PATCH api/v1/template-ui-elems/{id}
        [HttpPatch(ApiConstants.TemplateUIElemEndpoints.ById, Name = nameof(PatchTemplateUIElem))]
        [Authorize(Policy = ApiConstants.Policies.ManagementAccess)]
        [ProducesResponseType(typeof(TemplateUIElemDto), 200)]
        [ProducesResponseType(typeof(ResponseError), 400)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        [ProducesResponseType(typeof(ResponseError), 409)]
        public async Task<IActionResult> PatchTemplateUIElem(long id, [FromBody] PatchTemplateUIElemDto patchDto)
        {
            if (patchDto == null)
            {
                return BadRequest(JaCore.Api.Helpers.ErrorHelper.Validation("Patch document cannot be null."));
            }
            var result = await _templateUIElemService.PatchTemplateUIElemAsync(id, patchDto);
            return result.ToActionResult(this);
        }

        [HttpDelete(ApiConstants.TemplateUIElemEndpoints.ById, Name = nameof(DeleteTemplateUIElem))]
        [Authorize(Policy = ApiConstants.Policies.AdminAccess)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ResponseError), 400)]
        [ProducesResponseType(typeof(ResponseError), 404)]
        public async Task<IActionResult> DeleteTemplateUIElem(long id)
        {
            var result = await _templateUIElemService.DeleteTemplateUIElemAsync(id);
            return result.ToActionResult(this);
        }
    }
} 