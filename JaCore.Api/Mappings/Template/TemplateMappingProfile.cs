using AutoMapper;
using JaCore.Api.DTOs.Template;
using JaCore.Api.Entities.Template;

namespace JaCore.Api.Mappings.Template
{
    public class TemplateMappingProfile : Profile
    {
        public TemplateMappingProfile()
        {
            // TemplateUIElem Mappings
            CreateMap<TemplateUIElem, TemplateUIElemDto>().ReverseMap();
            CreateMap<CreateTemplateUIElemDto, TemplateUIElem>();
            CreateMap<UpdateTemplateUIElemDto, TemplateUIElem>();
        }
    }
} 