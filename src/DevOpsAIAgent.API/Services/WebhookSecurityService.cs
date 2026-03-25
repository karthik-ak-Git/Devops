using System.Security.Cryptography;
using System.Text;
using DevOpsAIAgent.Core.Interfaces.Services;

namespace DevOpsAIAgent.API.Services;

/// <summary>
/// Service for webhook security operations including HMAC verification
/// </summary>
public class WebhookSecurityService : IWebhookSecurityService
{
    private readonly ILogger<WebhookSecurityService> _logger;

    public WebhookSecurityService(ILogger<WebhookSecurityService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generate a secure webhook secret
    /// </summary>
    /// <returns>Base64 encoded random secret</returns>
    public string GenerateWebhookSecret()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32]; // 256-bit secret
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Verify GitHub webhook signature using HMAC-SHA256
    /// </summary>
    /// <param name="payload">Raw webhook payload</param>
    /// <param name="signatureHeader">X-Hub-Signature-256 header value</param>
    /// <param name="secret">Webhook secret</param>
    /// <returns>True if signature is valid</returns>
    public bool VerifyWebhookSignature(string payload, string signatureHeader, string secret)
    {
        try
        {
            if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(secret))
            {
                _logger.LogWarning("Missing required parameters for webhook signature verification");
                return false;
            }

            // GitHub signature format: sha256=<signature>
            if (!signatureHeader.StartsWith("sha256="))
            {
                _logger.LogWarning("Invalid signature header format: {SignatureHeader}", signatureHeader);
                return false;
            }

            var expectedSignature = signatureHeader["sha256=".Length..];
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var secretBytes = Encoding.UTF8.GetBytes(secret);

            using var hmac = new HMACSHA256(secretBytes);
            var computedHash = hmac.ComputeHash(payloadBytes);
            var computedSignature = Convert.ToHexString(computedHash).ToLowerInvariant();

            // Use constant-time comparison to prevent timing attacks
            var isValid = CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(expectedSignature),
                Convert.FromHexString(computedSignature));

            if (!isValid)
            {
                _logger.LogWarning("Webhook signature verification failed");
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying webhook signature");
            return false;
        }
    }

    /// <summary>
    /// Extract event type from X-GitHub-Event header
    /// </summary>
    /// <param name="eventHeader">X-GitHub-Event header value</param>
    /// <returns>Event type or "unknown" if invalid</returns>
    public string GetEventType(string eventHeader)
    {
        if (string.IsNullOrWhiteSpace(eventHeader))
        {
            _logger.LogWarning("Missing or empty GitHub event header");
            return "unknown";
        }

        var eventType = eventHeader.Trim().ToLowerInvariant();

        _logger.LogDebug("GitHub event type: {EventType}", eventType);

        return eventType;
    }
}