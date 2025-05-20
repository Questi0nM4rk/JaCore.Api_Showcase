using JaCore.Api.Data;
using JaCore.Api.Repositories.Abstractions.Template;
// If ITemplateRepository needs specific entities, add usings here, e.g.:
// using JaCore.Api.Entities.Template;

namespace JaCore.Api.Repositories.Template
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly ApplicationDbContext _context;

        public TemplateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Implement ITemplateRepository methods here
    }
} 