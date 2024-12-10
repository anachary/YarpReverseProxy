using System.Threading.RateLimiting;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Kestrel with enhanced security settings
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            // Connection limits
            serverOptions.Limits.MaxConcurrentConnections = 100;
            serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
            
            // Timeouts
            serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(120);
            serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
        });

        // Load the configuration from appsettings.json
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        var config = builder.Configuration.GetSection("ReverseProxy");

        // Add DDoS Protection Services
        builder.Services.AddSingleton<DDoSProtectionService>();
        
        // Add YARP services and configure them from appsettings.json
        builder.Services.AddReverseProxy()
            .LoadFromConfig(config);

        // Add rate limiting and DDoS protection
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });

        var app = builder.Build();

        // Use DDoS Protection Middleware
        app.UseMiddleware<DDoSProtectionMiddleware>();

        // Apply rate limiting
        app.UseRateLimiter();

        // Use YARP Middleware
        app.MapReverseProxy();

        app.Run();
    }
}
