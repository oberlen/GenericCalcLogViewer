using GenericCalcLogViewer.Configuration;

namespace GenericCalcLogViewer.Services;

/// <summary>
/// שירות ניהול תצורת סביבה
/// </summary>
public class EnvironmentService : IEnvironmentService
{
    private readonly Dictionary<string, EnvironmentConfiguration> _configurations;
    private readonly ILogger<EnvironmentService> _logger;

    public EnvironmentService(IConfiguration configuration, ILogger<EnvironmentService> logger)
    {
        _logger = logger;
        _configurations = new Dictionary<string, EnvironmentConfiguration>(StringComparer.OrdinalIgnoreCase)
        {
            ["DEV"] = new EnvironmentConfiguration
            {
                Environment = "DEV",
                ConnectionString = configuration.GetConnectionString("Gvia_DEV") ?? string.Empty,
                LogDirectory = configuration["LogDirectories:DEV"] ?? string.Empty
            },
            ["TEST"] = new EnvironmentConfiguration
            {
                Environment = "TEST",
                ConnectionString = configuration.GetConnectionString("Gvia_TEST") ?? string.Empty,
                LogDirectory = configuration["LogDirectories:TEST"] ?? string.Empty
            },
            ["INT"] = new EnvironmentConfiguration
            {
                Environment = "INT",
                ConnectionString = configuration.GetConnectionString("Gvia_INT") ?? string.Empty,
                LogDirectory = configuration["LogDirectories:INT"] ?? string.Empty
            },
            ["PROD"] = new EnvironmentConfiguration
            {
                Environment = "PROD",
                ConnectionString = configuration.GetConnectionString("Gvia_PROD") ?? string.Empty,
                LogDirectory = configuration["LogDirectories:PROD"] ?? string.Empty
            }
        };
    }

    public EnvironmentConfiguration GetConfiguration(string environment)
    {
        if (string.IsNullOrWhiteSpace(environment))
        {
            throw new ArgumentException("Environment cannot be null or empty", nameof(environment));
        }

        if (!_configurations.TryGetValue(environment, out var config))
        {
            throw new ArgumentException($"Unsupported environment: {environment}. " +
                $"Supported environments are: {string.Join(", ", _configurations.Keys)}");
        }

        return config;
    }
}
