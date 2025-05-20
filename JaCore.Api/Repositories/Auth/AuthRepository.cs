using JaCore.Api.Data;
using JaCore.Api.Repositories.Abstractions.Auth;
using Microsoft.Extensions.Logging;

namespace JaCore.Api.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(ApplicationDbContext context, ILogger<AuthRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Implement IAuthRepository methods here if any
    }
} 