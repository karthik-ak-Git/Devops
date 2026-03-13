# OpenAI Configuration Guide

## Standard OpenAI Setup

If using the standard OpenAI API:

1. Get your API key from: https://platform.openai.com/api-keys
2. Update `appsettings.Development.json`:

```json
{
  "OpenAI": {
    "ApiKey": "sk-proj-xxxxxxxxxxxxx",
    "Model": "gpt-4o",
    "AzureEndpoint": ""
  }
}
```

**Note:** Leave `AzureEndpoint` empty for standard OpenAI.

---

## Azure OpenAI Setup

If using Azure OpenAI Service:

1. Get your endpoint from Azure Portal (e.g., `https://your-resource.openai.azure.com/`)
2. Get your API key from Azure Portal
3. Update `appsettings.Development.json`:

```json
{
  "OpenAI": {
    "ApiKey": "your-azure-openai-key",
    "Model": "gpt-4o",
    "AzureEndpoint": "https://your-resource.openai.azure.com/"
  }
}
```

**Note:** The `Model` should match your deployed model name in Azure.

---

## Recommended Models

- **gpt-4o** - Latest GPT-4 Optimized model (recommended)
- **gpt-4-turbo** - Fast, capable model
- **gpt-4** - Most capable model (slower/more expensive)
- **gpt-3.5-turbo** - Budget option (less accurate for complex debugging)

---

## Security Best Practices

### For Production:
- Use Azure Key Vault or similar secret management
- Use environment variables instead of appsettings.json
- Enable User Secrets for local development:

```powershell
cd src\DevOpsAIAgent.Web
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "your-api-key-here"
dotnet user-secrets set "GitHub:PersonalAccessToken" "your-github-token-here"
```

### For Development:
- Keep `appsettings.Development.json` out of source control
- Add to `.gitignore` if not already present
