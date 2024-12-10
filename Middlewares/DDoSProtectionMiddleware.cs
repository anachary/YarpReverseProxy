
// DDoS Protection Middleware
public class DDoSProtectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly DDoSProtectionService _ddosProtectionService;
    private readonly ILogger<DDoSProtectionMiddleware> _logger;

    public DDoSProtectionMiddleware(
        RequestDelegate next, 
        DDoSProtectionService ddosProtectionService,
        ILogger<DDoSProtectionMiddleware> logger)
    {
        _next = next;
        _ddosProtectionService = ddosProtectionService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress;

        // Check if IP is blocked
        if (clientIp != null && _ddosProtectionService.IsBlocked(clientIp))
        {
            _logger.LogWarning($"Blocked request from IP: {clientIp}");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access denied");
            return;
        }

        try
        {
            // Track request
            _ddosProtectionService.TrackRequest(clientIp);

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
        finally
        {
            // Release tracking
            _ddosProtectionService.ReleaseRequest(clientIp);
        }
    }
}
