using Microsoft.AspNetCore.Mvc; // Required for ApiVersion

namespace JaCore.Api.Helpers;

/// <summary>
/// Constants specific to the JaCore.Api project.
/// </summary>
public static class ApiConstants
{
    /// <summary>
    /// API Version constants.
    /// </summary>
    public static class Versions
    {
        public const string VersionString = "1.0";
        public static readonly ApiVersion Version = new(1, 0);
        // Add other versions here, e.g.:
        // public const string V2_0_String = "2.0";
        // public static readonly ApiVersion V2_0 = new(2, 0);
    }

    /// <summary>
    /// Base route prefix including version placeholder.
    /// </summary>
    private const string RoutePrefixBase = $"api/v{Versions.VersionString}"; // Keep version placeholder for AddApiVersioning

    /// <summary>
    /// Standardized ID parameter placeholders for routes.
    /// </summary>
    public static class IdParams
    {
        // Auth/User can keep their specific ID types
        public const string UserId = "{userId:guid}"; // Assuming UserController uses userId

        // Device Domain Entities - Standardized to long IDs and specific names
        public const string DeviceId = "{deviceId:long}";
        public const string CardId = "{cardId:long}";
        public const string OperationId = "{opId:long}"; // As per devicedomain.md for DeviceOperation
        public const string EventId = "{eventId:long}";
        public const string LocationId = "{locationId:long}";
        public const string SupplierId = "{supplierId:long}";
        public const string ServiceId = "{serviceId:long}";
        public const string MetConfirmationId = "{metConfirmationId:long}";
        public const string CategoryId = "{categoryId:long}"; // Assuming 'Categories' is a distinct entity
        public const string TemplateUIElemId = "{templateUIElemId:long}";

        // Common generic placeholders
        public const string Name = "{name}";
        public const string SerialNumber = "{serialNumber}";
    }

    public static class BasePaths
    {
        // Authentication & User Management
        public const string Auth = $"{RoutePrefixBase}/auth";
        public const string Users = $"{RoutePrefixBase}/users";
        public const string UserInstance = $"{Users}/{IdParams.UserId}";

        // --- Device Module Top-Level Entities ---
        public const string Device = $"{RoutePrefixBase}/device"; // Singular form
        public const string DeviceInstance = $"{Device}/{IdParams.DeviceId}";

        public const string Location = $"{RoutePrefixBase}/location"; // Singular form
        public const string LocationInstance = $"{Location}/{IdParams.LocationId}";

        public const string Supplier = $"{RoutePrefixBase}/supplier"; // Singular form
        public const string SupplierInstance = $"{Supplier}/{IdParams.SupplierId}";

        public const string Service = $"{RoutePrefixBase}/service"; // Singular form
        public const string ServiceInstance = $"{Service}/{IdParams.ServiceId}";

        public const string MetConfirmation = $"{RoutePrefixBase}/metconfirmation"; // Singular form
        public const string MetConfirmationInstance = $"{MetConfirmation}/{IdParams.MetConfirmationId}";
        
        public const string TemplateUIElem = $"{RoutePrefixBase}/template-ui-elem"; // Singular form
        public const string TemplateUIElemInstance = $"{TemplateUIElem}/{IdParams.TemplateUIElemId}";

        public const string Category = $"{RoutePrefixBase}/category"; // Singular form, assuming 'Categories' was this
        public const string CategoryInstance = $"{Category}/{IdParams.CategoryId}";

        // --- Device Module Nested Entities ---
        // DeviceCard: Nested under a specific Device
        public const string DeviceCardCollection = $"{DeviceInstance}/cards"; // Plural "cards" for collection
        public const string DeviceCardInstance = $"{DeviceCardCollection}/{IdParams.CardId}";

        // DeviceOperation: Nested under a specific DeviceCard
        public const string DeviceOperationCollection = $"{DeviceCardInstance}/operations"; // Plural "operations"
        public const string DeviceOperationInstance = $"{DeviceOperationCollection}/{IdParams.OperationId}";

        // Event: Nested under a specific DeviceCard
        public const string EventCollection = $"{DeviceCardInstance}/events"; // Plural "events"
        public const string EventInstance = $"{EventCollection}/{IdParams.EventId}";
        
        // If DeviceCards also have a global (non-nested) context, e.g., for serial lookup
        public const string DeviceCardsGlobal = $"{RoutePrefixBase}/device-cards";
    }

    // Authentication and Authorization related constants
    public static class AuthEndpoints
    {
        public const string Register = "register";
        public const string Login = "login";
        public const string Refresh = "refresh";
        public const string Logout = "logout";
        public const string AdminOnlyData = "admin-only";
    }

    public static class AuthRoutes
    {
        public const string HealthCheck = $"{RoutePrefixBase}/healthz"; // Keep health check separate
        public const string Register = $"{BasePaths.Auth}/{AuthEndpoints.Register}";
        public const string Login = $"{BasePaths.Auth}/{AuthEndpoints.Login}";
        public const string Refresh = $"{BasePaths.Auth}/{AuthEndpoints.Refresh}";
        public const string Logout = $"{BasePaths.Auth}/{AuthEndpoints.Logout}";
        public const string AdminOnly = $"{BasePaths.Auth}/{AuthEndpoints.AdminOnlyData}";
    }

    // User management related constants
    public static class UserEndpoints
    {
        public const string ById = IdParams.UserId; // Using standardized param
        public const string UpdateRoles = $"{ById}/roles";
        public const string Me = "me";
        public const string Delete = ById;
        public const string Update = ById;
    }

    public static class UserRoutes
    {
        public const string GetAll = BasePaths.Users;
        public const string GetById = BasePaths.UserInstance;
        public const string Update = BasePaths.UserInstance; // PUT to /users/{userId}
        public const string Me = $"{BasePaths.Users}/{UserEndpoints.Me}";
        public const string UpdateRoles = $"{BasePaths.UserInstance}/roles"; // Path uses UserInstance
        public const string Delete = BasePaths.UserInstance; // DELETE to /users/{userId}
    }

    // --- Device Module Routes --- 

    public static class DeviceEndpoints
    {
        public const string ById = IdParams.DeviceId; // This is just the ID parameter itself
        public const string BySerial = $"serial/{IdParams.SerialNumber}";
        public const string ByName = $"name/{IdParams.Name}";
        public const string ByLocation = $"by-location/{IdParams.LocationId}"; // Refers to Location's ID
        public const string LinkLocation = "location"; // Action on a device instance
        public const string Disable = "disable";     // Action on a device instance
        public const string Enable = "enable";       // Action on a device instance
        public const string UpdateStatus = "status"; // Action on a device instance (example)
    }
    public static class DeviceRoutes
    {
        public const string GetAll = BasePaths.Device;
        public const string Create = BasePaths.Device;
        public const string GetById = BasePaths.DeviceInstance;
        public const string GetBySerial = $"{BasePaths.Device}/{DeviceEndpoints.BySerial}";
        public const string GetByName = $"{BasePaths.Device}/{DeviceEndpoints.ByName}";
        public const string GetByLocation = $"{BasePaths.Device}/{DeviceEndpoints.ByLocation}"; // Search devices by location
        public const string Update = BasePaths.DeviceInstance;
        public const string PartialUpdate = BasePaths.DeviceInstance;
        public const string Delete = BasePaths.DeviceInstance;
        public const string LinkLocation = $"{BasePaths.DeviceInstance}/{DeviceEndpoints.LinkLocation}";
        public const string Disable = $"{BasePaths.DeviceInstance}/{DeviceEndpoints.Disable}";
        public const string Enable = $"{BasePaths.DeviceInstance}/{DeviceEndpoints.Enable}";
        public const string UpdateStatus = $"{BasePaths.DeviceInstance}/{DeviceEndpoints.UpdateStatus}";
    }

    public static class DeviceCardEndpoints
    {
        public const string ById = IdParams.CardId; // This is just the ID parameter itself
        // For operations on the collection (BasePaths.DeviceCardCollection)
        public const string GetAll = ""; // GET "" on the collection path
        public const string Create = ""; // POST "" on the collection path
        // Global specific endpoint (if any)
        public const string GlobalGetBySerial = $"serial/{IdParams.SerialNumber}"; // Assuming card has a serial or found by device's serial
    }
    public static class DeviceCardRoutes
    {
        public const string GetAllForDevice = BasePaths.DeviceCardCollection;
        public const string CreateForDevice = BasePaths.DeviceCardCollection;
        public const string GetById = BasePaths.DeviceCardInstance;
        public const string Update = BasePaths.DeviceCardInstance;
        public const string PartialUpdate = BasePaths.DeviceCardInstance;
        public const string Delete = BasePaths.DeviceCardInstance;

        public const string GlobalGetBySerial = $"{BasePaths.DeviceCardsGlobal}/{DeviceCardEndpoints.GlobalGetBySerial}";
    }

    public static class CategoryEndpoints
    {
        public const string ById = IdParams.CategoryId;
    }
    public static class CategoryRoutes
    {
        public const string GetAll = BasePaths.Category;
        public const string Create = BasePaths.Category;
        public const string GetById = BasePaths.CategoryInstance;
        public const string Update = BasePaths.CategoryInstance;
        public const string Delete = BasePaths.CategoryInstance;
    }

    public static class SupplierEndpoints
    {
        public const string ById = IdParams.SupplierId;
        public const string ByName = $"name/{IdParams.Name}"; // Added for consistency if GetByName exists
    }
    public static class SupplierRoutes
    {
        public const string GetAll = BasePaths.Supplier;
        public const string Create = BasePaths.Supplier;
        public const string GetById = BasePaths.SupplierInstance;
        public const string GetByName = $"{BasePaths.Supplier}/{SupplierEndpoints.ByName}";
        public const string Update = BasePaths.SupplierInstance;
        public const string PartialUpdate = BasePaths.SupplierInstance;
        public const string Delete = BasePaths.SupplierInstance;
    }

    public static class ServiceEndpoints
    {
        public const string ById = IdParams.ServiceId;
        public const string ByName = $"name/{IdParams.Name}";  // Added for consistency if GetByName exists
    }
    public static class ServiceRoutes
    {
        public const string GetAll = BasePaths.Service;
        public const string Create = BasePaths.Service;
        public const string GetById = BasePaths.ServiceInstance;
        public const string GetByName = $"{BasePaths.Service}/{ServiceEndpoints.ByName}";
        public const string Update = BasePaths.ServiceInstance;
        public const string PartialUpdate = BasePaths.ServiceInstance;
        public const string Delete = BasePaths.ServiceInstance;
    }

    public static class EventEndpoints
    {
        public const string ById = IdParams.EventId;
        public const string GetAll = ""; // GET "" on the collection path (BasePaths.EventCollection)
        public const string Create = ""; // POST "" on the collection path
    }
    public static class EventRoutes
    {
        public const string GetAllForDeviceCard = BasePaths.EventCollection;
        public const string CreateForDeviceCard = BasePaths.EventCollection;
        public const string GetById = BasePaths.EventInstance;
        public const string Delete = BasePaths.EventInstance; // Events are often immutable but can be deleted
    }

    public static class DeviceOperationEndpoints
    {
        public const string ById = IdParams.OperationId;
        public const string GetAll = ""; // GET "" on the collection path
        public const string Create = ""; // POST "" on the collection path
        public const string Reorder = "reorder"; // Action on the collection
    }
    public static class DeviceOperationRoutes
    {
        public const string GetAllForDeviceCard = BasePaths.DeviceOperationCollection;
        public const string CreateForDeviceCard = BasePaths.DeviceOperationCollection;
        public const string GetById = BasePaths.DeviceOperationInstance;
        public const string Update = BasePaths.DeviceOperationInstance;
        public const string PartialUpdate = BasePaths.DeviceOperationInstance;
        public const string Delete = BasePaths.DeviceOperationInstance;
        public const string UpdateOrder = $"{BasePaths.DeviceOperationCollection}/{DeviceOperationEndpoints.Reorder}";
    }

    // --- End Device Module Routes ---

    // Location Routes
    public static class LocationEndpoints
    {
        public const string ById = IdParams.LocationId;
        public const string ByName = $"name/{IdParams.Name}";
    }
    public static class LocationRoutes
    {
        public const string GetAll = BasePaths.Location;
        public const string Create = BasePaths.Location;
        public const string GetById = BasePaths.LocationInstance;
        public const string GetByName = $"{BasePaths.Location}/{LocationEndpoints.ByName}";
        public const string Update = BasePaths.LocationInstance;
        public const string PartialUpdate = BasePaths.LocationInstance; // Added for consistency
        public const string Delete = BasePaths.LocationInstance;
    }

    // MetConfirmation Routes
    public static class MetConfirmationEndpoints
    {
        public const string ById = IdParams.MetConfirmationId;
        public const string ByName = $"name/{IdParams.Name}"; // Added for consistency if GetByName exists
    }
    public static class MetConfirmationRoutes
    {
        public const string GetAll = BasePaths.MetConfirmation;
        public const string Create = BasePaths.MetConfirmation;
        public const string GetById = BasePaths.MetConfirmationInstance;
        public const string GetByName = $"{BasePaths.MetConfirmation}/{MetConfirmationEndpoints.ByName}";
        public const string Update = BasePaths.MetConfirmationInstance;
        public const string PartialUpdate = BasePaths.MetConfirmationInstance; // Added
        public const string Delete = BasePaths.MetConfirmationInstance;
    }

    // TemplateUIElem Routes
    public static class TemplateUIElemEndpoints
    {
        public const string ById = IdParams.TemplateUIElemId;
        public const string ByName = $"name/{IdParams.Name}";
    }
    public static class TemplateUIElemRoutes
    {
        public const string GetAll = BasePaths.TemplateUIElem;
        public const string Create = BasePaths.TemplateUIElem;
        public const string GetById = BasePaths.TemplateUIElemInstance;
        public const string GetByName = $"{BasePaths.TemplateUIElem}/{TemplateUIElemEndpoints.ByName}";
        public const string Update = BasePaths.TemplateUIElemInstance;
        public const string PartialUpdate = BasePaths.TemplateUIElemInstance;
        public const string Delete = BasePaths.TemplateUIElemInstance;
    }

    /// <summary>
    /// Authorization policy names used within the API.
    /// </summary>
    public static class Policies
    {
        public const string AuthenticatedUser = "AuthenticatedUser";
        public const string ManageAccess = "ManageAccess";
        public const string AdminOnly = "AdminOnly"; // Consider if this is distinct from AdminAccess
        public const string AdminAccess = "AdminAccess";
        public const string ManagementAccess = "ManagementAccess"; // Often ManageAccess is preferred for consistency
        public const string UserAccess = "UserAccess";
        public const string AuthBased = "AuthBased"; // Vague, consider specific policies
    }

    /// <summary>
    /// Configuration keys for JWT settings.
    /// </summary>
    public static class JwtConfigKeys
    {
        public const string Section = "Jwt";
        public const string Issuer = $"{Section}:Issuer";
        public const string Audience = $"{Section}:Audience";
        public const string Secret = $"{Section}:Secret";
        public const string AccessExpiryMinutes = $"{Section}:AccessTokenExpirationMinutes";
        public const string RefreshExpiryDays = $"{Section}:RefreshTokenExpirationDays";
    }

    /// <summary>
    /// Common query parameter names.
    /// </summary>
    public static class QueryParams
    {
        public const string PageNumber = "pageNumber";
        public const string PageSize = "pageSize";
    }
}
