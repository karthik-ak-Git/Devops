using DevOpsAIAgent.Core.Interfaces.Services;
using Mscc.GenerativeAI;

namespace DevOpsAIAgent.Web.Services;

public class GeminiAssistantService : IAIAssistantService
{
    private readonly ILogger<GeminiAssistantService> _logger;
    private readonly string _apiKey;
    private readonly string _modelName;
    private readonly bool _isConfigured;

    public GeminiAssistantService(ILogger<GeminiAssistantService> logger)
    {
        _logger = logger;
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty;
        _modelName = Environment.GetEnvironmentVariable("GEMINI_MODEL") ?? "gemini-2.0-flash-exp";
        _isConfigured = !string.IsNullOrWhiteSpace(_apiKey);

        if (!_isConfigured)
            _logger.LogWarning("GEMINI_API_KEY not set. AI analysis disabled.");
        else
            _logger.LogInformation("Gemini AI initialized with model: {Model}", _modelName);
    }

    public async Task<string> AnalyzeFailureAsync(string errorLog, string gitDiff)
    {
        if (!_isConfigured)
            return FormatError("GEMINI_API_KEY not configured. Set it in your .env file.");

        try
        {
            var googleAI = new GoogleAI(apiKey: _apiKey);
            var model = googleAI.GenerativeModel(model: _modelName);

            var prompt = $@"You are an expert DevOps engineer specializing in CI/CD pipeline debugging.

Analyze the following CI/CD failure and provide:
1. A clear **Root Cause Analysis**
2. The exact **code fix** needed
3. A brief **Explanation** of the fix

Format in Markdown:

## Root Cause Analysis
[What went wrong and why]

## Recommended Fix
```
// The exact code change needed
```

## Explanation
[Why this fix resolves the issue]

---

## Git Diff (Changes that triggered the failure)
```diff
{gitDiff}
```

## Error Log
```
{errorLog}
```

Be specific, actionable, and provide copy-paste ready fixes.";

            var response = await model.GenerateContent(prompt);
            if (response?.Text == null)
                return FormatError("Gemini returned an empty response.");

            _logger.LogInformation("AI analysis completed ({Length} chars)", response.Text.Length);
            return response.Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gemini AI analysis failed");
            return FormatError(ex.Message);
        }
    }

    public async Task<string> SuggestIncidentResolutionAsync(string incidentDescription, IEnumerable<string> similarPastAnalyses)
    {
        if (!_isConfigured)
            return FormatError("GEMINI_API_KEY not configured.");

        try
        {
            var googleAI = new GoogleAI(apiKey: _apiKey);
            var model = googleAI.GenerativeModel(model: _modelName);

            var pastContext = string.Join("\n---\n", similarPastAnalyses.Take(3));
            var prompt = $@"You are a DevOps incident resolution expert.

Given this incident and similar past resolutions, suggest a resolution plan.

## Current Incident
{incidentDescription}

## Similar Past Resolutions
{(string.IsNullOrWhiteSpace(pastContext) ? "No similar incidents found." : pastContext)}

Provide a concise resolution plan in Markdown with:
1. **Immediate Actions** - Steps to take now
2. **Root Cause** - Most likely cause based on patterns
3. **Prevention** - How to prevent recurrence";

            var response = await model.GenerateContent(prompt);
            return response?.Text ?? FormatError("Empty response from AI.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI incident resolution suggestion failed");
            return FormatError(ex.Message);
        }
    }

    private static string FormatError(string message) =>
        $"## AI Analysis Unavailable\n\n{message}\n\nPlease review the error log and git diff manually.";
}
