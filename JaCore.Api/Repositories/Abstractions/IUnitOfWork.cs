using JaCore.Api.Repositories.Abstractions.Auth;
using JaCore.Api.Repositories.Abstractions.Device;
using JaCore.Api.Repositories.Abstractions.Template;
using JaCore.Api.Repositories.Abstractions.User; // Assuming IUserManagementRepository is here
using Microsoft.EntityFrameworkCore.Storage; // Added for IDbContextTransaction
// Add other domain repository interfaces here as they are created
// e.g., using JaCore.Api.Repositories.Abstractions.Template;
// e.g., using JaCore.Api.Repositories.Abstractions.Work;

namespace JaCore.Api.Repositories.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepository Auth { get; }
        IUserManagementRepository Users { get; }
        IDeviceRepository Devices { get; }
        IDeviceCardRepository DeviceCards { get; }
        ILocationRepository Locations { get; }
        IServiceEntityRepository ServiceEntities { get; }
        ISupplierRepository Suppliers { get; }
        IEventRepository Events { get; }
        IDeviceOperationRepository DeviceOperations { get; }
        ITemplateRepository Templates { get; }
        ITemplateUIElemRepository TemplateUIElems { get; }
        IMetConfirmationRepository MetConfirmations { get; }

        // Template Repositories (placeholder for now)
        // ITemplateUIElemRepository TemplateUIElems { get; }

        // Add other domain repositories here
        // ITemplateRepository Templates { get; }
        // IWorkOperationRepository WorkOperations { get; }

        Task<int> CompleteAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
} 