using JaCore.Api.Entities.Identity; // Assuming User entity might be relevant
using System.Threading.Tasks;

namespace JaCore.Api.Repositories.Abstractions.User
{
    public interface IUserManagementRepository
    {
        // Define methods relevant to user management, e.g.:
        // Task<User> GetUserByIdAsync(string userId);
        // Task<bool> CreateUserAsync(User user, string password);
        // Task<bool> UpdateUserAsync(User user);
        // ... etc. For now, leave empty to satisfy UnitOfWork reference.
    }
} 