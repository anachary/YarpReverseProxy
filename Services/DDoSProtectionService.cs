
using System.Collections.Concurrent;
using System.Net;
// DDoS Protection Service
public class DDoSProtectionService
{
    private readonly ConcurrentDictionary<IPAddress, ClientRequestInfo> _requestTracker 
        = new ConcurrentDictionary<IPAddress, ClientRequestInfo>();
    
    private readonly HashSet<IPAddress> _blockedIPs 
        = new HashSet<IPAddress>();

    private readonly int _maxRequestsPerMinute = 100;
    private readonly int _blockDuration = 15; // minutes

    public void TrackRequest(IPAddress? clientIp)
    {
        if (clientIp == null) return;

        var clientInfo = _requestTracker.GetOrAdd(clientIp, 
            _ => new ClientRequestInfo());

        lock (clientInfo)
        {
            // Remove old requests
            clientInfo.Requests.RemoveAll(r => 
                DateTime.UtcNow - r > TimeSpan.FromMinutes(1));

            clientInfo.Requests.Add(DateTime.UtcNow);

            // Check if requests exceed threshold
            if (clientInfo.Requests.Count > _maxRequestsPerMinute)
            {
                BlockIP(clientIp);
            }
        }
    }

    public void ReleaseRequest(IPAddress? clientIp)
    {
        if (clientIp == null) return;

        // Implement any cleanup logic if needed
    }

    public bool IsBlocked(IPAddress? clientIp)
    {
        if (clientIp == null) return false;

        return _blockedIPs.Contains(clientIp);
    }

    private void BlockIP(IPAddress clientIp)
    {
        // Add to blocked list
        _blockedIPs.Add(clientIp);

        // Schedule unblock
        Task.Delay(TimeSpan.FromMinutes(_blockDuration))
            .ContinueWith(_ => _blockedIPs.Remove(clientIp));
    }
}
