using JaCore.Api.Data;
using JaCore.Api.Repositories.Abstractions.User;
using Microsoft.Extensions.Logging;

namespace JaCore.Api.Repositories.User // Will create directory if it doesn't exist
{
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserManagementRepository> _logger;

        public UserManagementRepository(ApplicationDbContext context, ILogger<UserManagementRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Implement IUserManagementRepository methods here
    }
} 