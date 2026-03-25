using Refit;

namespace DevOpsAIAgent.API.Services.LlmProviders;

/// <summary>
/// Refit interface for OpenRouter API
/// </summary>
public interface IOpenRouterApi
{
    [Post("/chat/completions")]
    Task<OpenRouterResponse> CreateCompletionAsync(
        [Body] OpenRouterRequest request,
        [Header("Authorization")] string authorization,
        [Header("HTTP-Referer")] string? referer = null,
        [Header("X-Title")] string? title = null);
}

/// <summary>
/// OpenRouter API request model
/// </summary>
public class OpenRouterRequest
{
    public string Model { get; set; } = string.Empty;
    public OpenRouterMessage[] Messages { get; set; } = Array.Empty<OpenRouterMessage>();
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 4096;
}

/// <summary>
/// OpenRouter message model
/// </summary>
public class OpenRouterMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// OpenRouter API response model
/// </summary>
public class OpenRouterResponse
{
    public string Id { get; set; } = string.Empty;
    public string Object { get; set; } = string.Empty;
    public long Created { get; set; }
    public string Model { get; set; } = string.Empty;
    public OpenRouterChoice[] Choices { get; set; } = Array.Empty<OpenRouterChoice>();
    public OpenRouterUsage? Usage { get; set; }
}

/// <summary>
/// OpenRouter choice model
/// </summary>
public class OpenRouterChoice
{
    public int Index { get; set; }
    public OpenRouterMessage Message { get; set; } = new();
    public string FinishReason { get; set; } = string.Empty;
}

/// <summary>
/// OpenRouter usage model
/// </summary>
public class OpenRouterUsage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}