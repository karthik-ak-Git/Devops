using DevOpsAIAgent.Core.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace DevOpsAIAgent.Web.Services;

public class WebhookSecurityService : IWebhookSecurityService
{
    private readonly ILogger<WebhookSecurityService> _logger;

    public WebhookSecurityService(ILogger<WebhookSecurityService> logger) => _logger = logger;

    public string GenerateWebhookSecret()
    {
        var secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        _logger.LogDebug("Generated new webhook secret");
        return secret;
    }

    public bool VerifyWebhookSignature(string payload, string signatureHeader, string secret)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(signatureHeader) || !signatureHeader.StartsWith("sha256="))
            {
                _logger.LogWarning("Invalid signature header format");
                return false;
            }

            var receivedSignature = signatureHeader["sha256=".Length..];
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var computedSignature = Convert.ToHexString(computedHash).ToLowerInvariant();

            var isValid = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(receivedSignature.ToLowerInvariant()),
                Encoding.UTF8.GetBytes(computedSignature));

            if (!isValid)
                _logger.LogWarning("Webhook signature verification failed");

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying webhook signature");
            return false;
        }
    }

    public string GetEventType(string eventHeader) =>
        eventHeader?.ToLowerInvariant() ?? "unknown";
}
