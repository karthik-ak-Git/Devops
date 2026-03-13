using Mscc.GenerativeAI;

namespace DevOpsAIAgent.Web.Services;

/// <summary>
/// Implementation of AI Assistant Service using Google Gemini.
/// </summary>
public class GeminiAssistantService : IAIAssistantService
{
    private readonly ILogger<GeminiAssistantService> _logger;
    private readonly string _apiKey;
    private readonly string _modelName;
    private readonly bool _isConfigured;
    private readonly string? _configurationError;

    public GeminiAssistantService(ILogger<GeminiAssistantService> logger)
    {
        _logger = logger;

        // Load API key from environment variable
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _isConfigured = false;
            _configurationError = "GEMINI_API_KEY environment variable is not set. Please configure it in your .env file.";
            _logger.LogWarning(_configurationError);
            _modelName = "gemini-2.0-flash-exp"; // Default even when not configured
        }
        else
        {
            _isConfigured = true;
            // Load model name from environment variable or use default
            _modelName = Environment.GetEnvironmentVariable("GEMINI_MODEL") ?? "gemini-2.0-flash-exp";
            _logger.LogInformation("Gemini Assistant Service initialized with model: {Model}", _modelName);
        }
    }

    /// <summary>
    /// Analyzes a CI/CD failure using Gemini AI to determine root cause and suggest fixes.
    /// </summary>
    public async Task<string> AnalyzeFailureAsync(string errorLog, string gitDiff)
    {
        if (!_isConfigured)
        {
            _logger.LogWarning("Cannot analyze failure: {Error}", _configurationError);
            return GenerateErrorResponse(_configurationError ?? "Gemini AI not configured");
        }

        _logger.LogInformation("Starting Gemini AI analysis of CI/CD failure");

        try
        {
            // Initialize Gemini client and model
            var googleAI = new GoogleAI(apiKey: _apiKey);
            var model = googleAI.GenerativeModel(model: _modelName);

            // Construct the prompt
            var prompt = BuildAnalysisPrompt(errorLog, gitDiff);

            _logger.LogDebug("Sending request to Gemini model: {Model}", _modelName);
            _logger.LogDebug("Prompt length: {Length} characters", prompt.Length);

            // Generate content
            var response = await model.GenerateContent(prompt);

            if (response?.Text == null)
            {
                _logger.LogWarning("Gemini returned null or empty response");
                return GenerateErrorResponse("Gemini returned an empty response");
            }

            var analysisText = response.Text;

            _logger.LogInformation("Gemini analysis completed successfully. Response length: {Length} characters",
                analysisText.Length);

            return analysisText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Gemini AI analysis");
            return GenerateErrorResponse(ex.Message);
        }
    }

    /// <summary>
    /// Builds the analysis prompt for Gemini.
    /// </summary>
    private string BuildAnalysisPrompt(string errorLog, string gitDiff)
    {
        return $@"You are an expert DevOps engineer and Senior C#/.NET developer specializing in CI/CD pipeline debugging and automated code fixes.

**Your Task:**
Analyze the following CI/CD build/test failure and provide:
1. A clear, concise **Root Cause Analysis** explaining what went wrong and why
2. The exact **C# code snippet** that needs to be changed to fix the issue
3. A brief **Explanation** of why this fix resolves the problem

**Format your response in Markdown with this structure:**

## Root Cause Analysis
[Explain what went wrong and why - be specific about the error]

## Recommended Fix
```csharp
// Provide the exact C# code snippet that needs to be changed
// Include context: show the problematic code and the fixed version
```

## Explanation
[Brief explanation of why this fix resolves the issue and prevents future occurrences]

---

## Git Diff (Changes that triggered the failure)
```diff
{gitDiff}
```

---

## Error Log (From the failed workflow run)
```
{errorLog}
```

---

**Instructions:**
- Be specific and actionable
- Focus on the most likely root cause based on the evidence
- Provide copy-paste ready code fixes
- If multiple issues exist, prioritize the critical one
- Use C# best practices and .NET conventions
";
    }

    /// <summary>
    /// Generates an error response when AI analysis fails.
    /// </summary>
    private string GenerateErrorResponse(string errorMessage)
    {
        return $@"## Error During AI Analysis

An error occurred while analyzing the failure with Gemini AI:

```
{errorMessage}
```

### Manual Review Required

Please review the error log and git diff manually. Common issues to check:

1. **Syntax Errors** - Missing semicolons, brackets, or quotes
2. **Null Reference** - Check for null checks on objects
3. **Type Mismatches** - Verify variable types and conversions
4. **Missing Dependencies** - Ensure NuGet packages are restored
5. **Configuration Issues** - Check appsettings.json and environment variables

### Troubleshooting Steps:
1. Review the git diff for obvious syntax errors
2. Check the error log for stack traces
3. Verify all dependencies are installed
4. Run the build locally to reproduce the issue
5. Check for breaking changes in updated packages
";
    }
}
