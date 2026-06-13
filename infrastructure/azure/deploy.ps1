# Azure deployment script for TicketAlert
# Requirements: Azure CLI (logged in: az login)

param(
    [string]$Environment = "prod",
    [string]$Location = "norwayeast",
    [string]$AppName = "ticketalert",
    [string]$AdminPassword = "",
    [string]$JwtKey = "",
    [string]$TicketmasterApiKey = "",
    [string]$StripeSecretKey = "",
    [string]$StripeWebhookSecret = "",
    [string]$SmtpHost = "",
    [string]$SmtpUsername = "",
    [string]$SmtpPassword = ""
)

if (-not $AdminPassword) { $AdminPassword = Read-Host "PostgreSQL admin password" -AsSecureString }
if (-not $JwtKey) { $JwtKey = Read-Host "JWT signing key (min 32 tegn)" -AsSecureString }

$rgName = "rg-$AppName-$Environment"

Write-Host "`n=== Deployer TicketAlert til Azure ===" -ForegroundColor Cyan
Write-Host "  Ressursgruppe: $rgName" -ForegroundColor Gray
Write-Host "  Lokasjon:      $Location" -ForegroundColor Gray
Write-Host "  App:           $AppName-api-$Environment" -ForegroundColor Gray
Write-Host ""

# Sjekk at Azure CLI er logget inn
az account show --output none
if ($LASTEXITCODE -ne 0) {
    Write-Host "Logger inn på Azure..." -ForegroundColor Yellow
    az login
}

Write-Host "Oppretter ressursgruppe..." -ForegroundColor Green
az group create --name $rgName --location $Location --output none

Write-Host "Deployer Bicep-skjema..." -ForegroundColor Green
$bicepPath = Join-Path $PSScriptRoot "..\bicep\main.bicep"

az deployment group create `
    --resource-group $rgName `
    --template-file $bicepPath `
    --parameters `
        appName=$AppName `
        environmentName=$Environment `
        location=$Location `
        adminPassword=$AdminPassword `
        jwtKey=$JwtKey `
        ticketmasterApiKey=$TicketmasterApiKey `
        stripeSecretKey=$StripeSecretKey `
        stripeWebhookSecret=$StripeWebhookSecret `
        smtpHost=$SmtpHost `
        smtpUsername=$SmtpUsername `
        smtpPassword=$SmtpPassword `
    --output table

if ($LASTEXITCODE -eq 0) {
    $apiUrl = "https://$AppName-api-$Environment.azurewebsites.net"
    Write-Host "`n=== Deployment fullført! ===" -ForegroundColor Cyan
    Write-Host "  API URL: $apiUrl" -ForegroundColor Green

    Write-Host "`nSett VITE_API_URL i Vercel til:" -ForegroundColor Yellow
    Write-Host "  $apiUrl" -ForegroundColor White

    Write-Host "`nFor å bygge og deploye backend-koden til App Service:" -ForegroundColor Yellow
    Write-Host "  cd backend/src/TicketAlert.Api" -ForegroundColor Gray
    Write-Host '  dotnet publish -c Release -o publish' -ForegroundColor Gray
    Write-Host "  az webapp deploy --resource-group $rgName --name $AppName-api-$Environment --src-path publish --type zip" -ForegroundColor Gray
} else {
    Write-Host "`nDeployment feilet. Sjekk feilmeldingene over." -ForegroundColor Red
}
