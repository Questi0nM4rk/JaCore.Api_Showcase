using JaCore.Api.Repositories.Abstractions;
using JaCore.Api.Repositories;
using JaCore.Api.Services.Abstractions.Device;
using JaCore.Api.Services.Device;
using JaCore.Api.Services.Abstractions.Template;
using JaCore.Api.Services.Template;
using Microsoft.Extensions.DependencyInjection;

namespace JaCore.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Device Domain Services
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IMetConfirmationService, MetConfirmationService>();
            services.AddScoped<IServiceEntityService, ServiceService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IDeviceCardService, DeviceCardService>();
            services.AddScoped<IDeviceOperationService, DeviceOperationService>();
            services.AddScoped<IEventService, EventService>();

            // Register Template Domain Services
            services.AddScoped<ITemplateUIElemService, TemplateUIElemService>();
            
            // Add other services as needed

            return services;
        }
    }
} 