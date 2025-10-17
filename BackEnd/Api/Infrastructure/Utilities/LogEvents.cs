using Api.Application.Services;

internal static class AppLogEvents
{
    internal static EventId Create = new(1000, "Created");
    internal static EventId Update = new(1001, "Updated");
    internal static EventId Delete = new(1002, "Deleted");
    internal static EventId Deactivate = new(1003, "Deactivated");
    internal static EventId Retrieve = new(1004, "Retrieved");
    internal static EventId Filter = new(1005, "Filtered");

    internal static void LogCreate(this ILogger logger, string entityName, object entityId) => logger.LogInformation(Create, "{Entity} with ID {Id} created.", entityName, entityId);
    internal static void LogUpdate(this ILogger logger, string entityName, object entityId) => logger.LogInformation(Update, "{Entity} with ID {Id} updated.", entityName, entityId);
    internal static void LogDelete(this ILogger logger, string entityName, object entityId) => logger.LogInformation(Delete, "{Entity} with ID {Id} deleted.", entityName, entityId);
    internal static void LogDeactivate(this ILogger logger, string entityName, object entityId) => logger.LogInformation(Deactivate, "{Entity} with ID {Id} deactivated.", entityName, entityId);
    internal static void LogRetrieve(this ILogger logger, string entityType, int entityCount) => logger.LogInformation(Retrieve, "{Count} {Entity} retrieved.", entityCount, entityType);
    internal static void LogFilter(this ILogger logger, string entityType, int entityCount) => logger.LogInformation(Filter, "{Count} {Entity} filtered.", entityCount, entityType);
}