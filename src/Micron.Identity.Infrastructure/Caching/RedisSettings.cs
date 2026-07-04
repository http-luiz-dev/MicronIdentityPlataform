namespace Micron.Identity.Infrastructure.Caching;

public class RedisSettings
{
    public const string SectionName = "RedisSettings";

    public string ConnectionString { get; set; } = string.Empty;

    public string InstanceName { get; set; } = "micron-identity:";
}
