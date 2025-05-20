using System.ComponentModel.DataAnnotations;

namespace JaCore.Api.DTOs.Template
{
    public class TemplateUIElemDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public int ElemType { get; set; }
    }

    public class CreateTemplateUIElemDto
    {
        [Required]
        [StringLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        public int ElemType { get; set; }
    }

    public class UpdateTemplateUIElemDto
    {
        [Required]
        [StringLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        public int ElemType { get; set; }
    }

    public class PatchTemplateUIElemDto
    {
        [StringLength(20)]
        public string? Name { get; set; }

        public int? ElemType { get; set; }
    }
} 