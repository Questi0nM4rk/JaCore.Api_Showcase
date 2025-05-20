using AutoMapper;
using JaCore.Api.DTOs.Device;
using JaCore.Api.Entities.Device;

namespace JaCore.Api.Mappings.Device
{
    public class DeviceMappingProfile : Profile
    {
        public DeviceMappingProfile()
        {
            // Location Mappings
            CreateMap<Location, LocationDto>();
            CreateMap<CreateLocationDto, Location>();
            CreateMap<UpdateLocationDto, Location>();
            CreateMap<PatchLocationDto, Location>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // MetConfirmation Mappings
            CreateMap<MetConfirmation, MetConfirmationDto>();
            CreateMap<CreateMetConfirmationDto, MetConfirmation>();
            CreateMap<UpdateMetConfirmationDto, MetConfirmation>();
            CreateMap<PatchMetConfirmationDto, MetConfirmation>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ServiceEntity (Service) Mappings
            CreateMap<ServiceEntity, ServiceDto>();
            CreateMap<CreateServiceDto, ServiceEntity>();
            CreateMap<UpdateServiceDto, ServiceEntity>();
            CreateMap<PatchServiceDto, ServiceEntity>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Supplier Mappings
            CreateMap<Supplier, SupplierDto>();
            CreateMap<CreateSupplierDto, Supplier>();
            CreateMap<UpdateSupplierDto, Supplier>();
            CreateMap<PatchSupplierDto, Supplier>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Device Mappings
            CreateMap<JaCore.Api.Entities.Device.Device, DeviceDto>();
            CreateMap<CreateDeviceDto, JaCore.Api.Entities.Device.Device>();
            CreateMap<UpdateDeviceDto, JaCore.Api.Entities.Device.Device>();
            CreateMap<PatchDeviceDto, JaCore.Api.Entities.Device.Device>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // DeviceCard Mappings
            CreateMap<DeviceCard, DeviceCardDto>();
            CreateMap<CreateDeviceCardDto, DeviceCard>();
            CreateMap<UpdateDeviceCardDto, DeviceCard>();
            CreateMap<PatchDeviceCardDto, DeviceCard>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // DeviceOperation Mappings
            CreateMap<DeviceOperation, DeviceOperationDto>();
            CreateMap<CreateDeviceOperationDto, DeviceOperation>();
            CreateMap<UpdateDeviceOperationDto, DeviceOperation>();
            CreateMap<PatchDeviceOperationDto, DeviceOperation>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Event Mappings
            CreateMap<Event, EventDto>();
            CreateMap<CreateEventDto, Event>();
            CreateMap<UpdateEventDto, Event>();
            CreateMap<PatchEventDto, Event>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
} 