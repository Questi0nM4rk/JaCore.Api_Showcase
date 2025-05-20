using JaCore.Api.Data;
using JaCore.Api.Repositories.Abstractions;
using JaCore.Api.Repositories.Abstractions.Auth;
using JaCore.Api.Repositories.Abstractions.Device;
using JaCore.Api.Repositories.Abstractions.Template;
using JaCore.Api.Repositories.Abstractions.User;
using JaCore.Api.Repositories.Auth;
using JaCore.Api.Repositories.Device;
using JaCore.Api.Repositories.Template;
using JaCore.Api.Repositories.User;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System; // Required for GC.SuppressFinalize
using System.Threading.Tasks;

namespace JaCore.Api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private bool _disposed = false;

        // Auth and User Repositories
        public IAuthRepository Auth { get; private set; }
        public IUserManagementRepository Users { get; private set; }

        // Device Repositories (lazy loaded)
        private IDeviceRepository? _devices;
        public IDeviceRepository Devices => _devices ??= new DeviceRepository(_context);

        private IDeviceCardRepository? _deviceCards;
        public IDeviceCardRepository DeviceCards => _deviceCards ??= new DeviceCardRepository(_context);

        private ILocationRepository? _locations;
        public ILocationRepository Locations => _locations ??= new LocationRepository(_context);

        private IServiceEntityRepository? _serviceEntities; // Backing field name matches new property name
        public IServiceEntityRepository ServiceEntities => _serviceEntities ??= new ServiceEntityRepository(_context);

        private ISupplierRepository? _suppliers;
        public ISupplierRepository Suppliers => _suppliers ??= new SupplierRepository(_context);

        private IEventRepository? _events;
        public IEventRepository Events => _events ??= new EventRepository(_context);

        private IDeviceOperationRepository? _deviceOperations;
        public IDeviceOperationRepository DeviceOperations => _deviceOperations ??= new DeviceOperationRepository(_context);
        
        private IMetConfirmationRepository? _metConfirmations;
        public IMetConfirmationRepository MetConfirmations => _metConfirmations ??= new MetConfirmationRepository(_context);

        // Template Repositories (lazy loaded)
        private ITemplateRepository? _templates; // Corrected backing field
        public ITemplateRepository Templates => _templates ??= new TemplateRepository(_context); // Corrected instantiation

        private ITemplateUIElemRepository? _templateUIElems;
        public ITemplateUIElemRepository TemplateUIElems => _templateUIElems ??= new TemplateUIElemRepository(_context);


        public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = logger;
            _loggerFactory = loggerFactory;

            // Initialize repositories that require specific loggers
            Auth = new AuthRepository(_context, _loggerFactory.CreateLogger<AuthRepository>());
            Users = new UserManagementRepository(_context, _loggerFactory.CreateLogger<UserManagementRepository>());
            
            // Lazy loaded properties (Devices, DeviceCards, Locations, ServiceEntities, Suppliers, Events, DeviceOperations, MetConfirmations, Templates, TemplateUIElems)
            // are initialized on first access. No need to initialize them in the constructor
            // unless they also have specific constructor dependencies like ILogger passed from loggerFactory.
            // For consistency with Auth and Users, if repositories need specific loggers, they should be passed here.
            // Assuming Device/* and Template/* repositories take only DbContext for now as per generated code.
        }

        public async Task<int> CompleteAsync() // Renamed from CommitAsync to match IUnitOfWork
        {
            _logger.LogInformation("Completing unit of work and saving changes.");
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving changes in UnitOfWork.");
                throw; // Re-throw the exception to be handled by higher layers
            }
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                    _logger.LogInformation("UnitOfWork disposed.");
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
} 