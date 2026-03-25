namespace DevOpsAIAgent.Core.Models.Configuration;

/// <summary>
/// Configuration settings for LLM providers
/// </summary>
public class LlmConfiguration
{
    public const string SectionName = "LlmProviders";

    /// <summary>
    /// Default provider to use when none specified
    /// </summary>
    public string DefaultProvider { get; set; } = "Gemini";

    /// <summary>
    /// Enable fallback to other providers if primary fails
    /// </summary>
    public bool EnableFallback { get; set; } = true;

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gemini provider configuration
    /// </summary>
    public GeminiConfiguration Gemini { get; set; } = new();

    /// <summary>
    /// OpenRouter provider configuration
    /// </summary>
    public OpenRouterConfiguration OpenRouter { get; set; } = new();

    /// <summary>
    /// Ollama provider configuration
    /// </summary>
    public OllamaConfiguration Ollama { get; set; } = new();
}

/// <summary>
/// Configuration for Google Gemini
/// </summary>
public class GeminiConfiguration
{
    /// <summary>
    /// Gemini API key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Model to use (e.g., "gemini-1.5-pro", "gemini-1.5-flash")
    /// </summary>
    public string Model { get; set; } = "gemini-1.5-pro";

    /// <summary>
    /// Enable this provider
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Max tokens for response
    /// </summary>
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// Temperature for response creativity (0.0 to 2.0)
    /// </summary>
    public double Temperature { get; set; } = 0.7;
}

/// <summary>
/// Configuration for OpenRouter
/// </summary>
public class OpenRouterConfiguration
{
    /// <summary>
    /// OpenRouter API key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Base URL for OpenRouter API
    /// </summary>
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";

    /// <summary>
    /// Model to use (e.g., "anthropic/claude-3.5-sonnet", "meta-llama/llama-3.1-8b-instruct")
    /// </summary>
    public string Model { get; set; } = "anthropic/claude-3.5-sonnet";

    /// <summary>
    /// Enable this provider
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Max tokens for response
    /// </summary>
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// Temperature for response creativity (0.0 to 2.0)
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// App name for OpenRouter (optional)
    /// </summary>
    public string? AppName { get; set; } = "DevOpsAIAgent";
}

/// <summary>
/// Configuration for Ollama (local models)
/// </summary>
public class OllamaConfiguration
{
    /// <summary>
    /// Ollama server URL
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:11434";

    /// <summary>
    /// Model to use (e.g., "llama3.1", "codellama", "mistral")
    /// </summary>
    public string Model { get; set; } = "llama3.1";

    /// <summary>
    /// Enable this provider
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Temperature for response creativity (0.0 to 1.0)
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Keep model loaded in memory
    /// </summary>
    public bool KeepAlive { get; set; } = true;
}