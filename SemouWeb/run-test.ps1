# Starts the TEST copy of Semou Web on http://localhost:5090 (database Semou_Test).
# Everything saved here stays in Semou_Test; attachment uploads are simulated.
$ErrorActionPreference = 'Stop'
Set-Location "$PSScriptRoot\web"
npm run build | Out-Host
Set-Location "$PSScriptRoot\api"
dotnet build | Out-Host
$env:App__TestMode = 'true'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
Write-Host "Test copy running on http://localhost:5090  (Ctrl+C to stop)" -ForegroundColor Yellow
dotnet run --no-build --urls http://localhost:5090
