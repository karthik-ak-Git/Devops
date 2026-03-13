# Test Script for DevOps AI Agent Dashboard
# This script sends a test webhook payload to the local server

param(
    [string]$Url = "https://localhost:5001/api/webhooks/github",
    [string]$PayloadFile = "test-webhook-payload.json"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  DevOps AI Agent - Test Webhook" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if payload file exists
if (-not (Test-Path $PayloadFile)) {
    Write-Host "❌ Error: Payload file not found: $PayloadFile" -ForegroundColor Red
    Write-Host "   Creating a sample payload file..." -ForegroundColor Yellow
    
    $samplePayload = @{
        action = "completed"
        workflow_run = @{
            id = Get-Random -Minimum 100000000 -Maximum 999999999
            name = "CI/CD Pipeline"
            head_sha = -join ((1..40) | ForEach-Object { '{0:x}' -f (Get-Random -Maximum 16) })
            status = "completed"
            conclusion = "failure"
            html_url = "https://github.com/testorg/test-repo/actions/runs/123456"
            run_number = Get-Random -Minimum 1 -Maximum 100
            run_attempt = 1
        }
        repository = @{
            name = "test-repo"
            full_name = "testorg/test-repo"
            html_url = "https://github.com/testorg/test-repo"
            owner = @{
                login = "testorg"
                type = "Organization"
            }
        }
    }
    
    $samplePayload | ConvertTo-Json -Depth 10 | Out-File -FilePath $PayloadFile -Encoding UTF8
    Write-Host "✓ Created sample payload: $PayloadFile" -ForegroundColor Green
    Write-Host ""
}

# Read the payload
Write-Host "📄 Reading payload from: $PayloadFile" -ForegroundColor White
$payload = Get-Content $PayloadFile -Raw

# Display payload summary
$payloadObj = $payload | ConvertFrom-Json
Write-Host "   Repository: $($payloadObj.repository.full_name)" -ForegroundColor Gray
Write-Host "   Workflow: $($payloadObj.workflow_run.name)" -ForegroundColor Gray
Write-Host "   Commit: $($payloadObj.workflow_run.head_sha.Substring(0,7))" -ForegroundColor Gray
Write-Host "   Status: $($payloadObj.workflow_run.conclusion.ToUpper())" -ForegroundColor Red
Write-Host ""

# Send the request
Write-Host "🚀 Sending webhook to: $Url" -ForegroundColor White

try {
    $response = Invoke-RestMethod -Uri $Url `
        -Method Post `
        -ContentType "application/json" `
        -Body $payload `
        -SkipCertificateCheck

    Write-Host "✅ Webhook sent successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Response:" -ForegroundColor Cyan
    Write-Host ($response | ConvertTo-Json -Depth 10) -ForegroundColor Gray
    Write-Host ""
    Write-Host "✓ Check your dashboard at: https://localhost:5001" -ForegroundColor Green
}
catch {
    Write-Host "❌ Error sending webhook:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  1. Ensure the application is running (dotnet run)" -ForegroundColor Yellow
    Write-Host "  2. Check the URL is correct: $Url" -ForegroundColor Yellow
    Write-Host "  3. Verify SSL certificate (use -SkipCertificateCheck for local testing)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
