namespace DevOpsAIAgent.Core.Interfaces.Services;

public interface IWebhookSecurityService
{
    string GenerateWebhookSecret();
    bool VerifyWebhookSignature(string payload, string signatureHeader, string secret);
    string GetEventType(string eventHeader);
}
