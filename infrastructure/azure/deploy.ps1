# Azure deployment script for TicketAlert
# Requires: Azure CLI, Bicep CLI

param(
    [string]$Environment = "prod",
    [string]$Location = "norwayeast",
    [string]$AppName = "ticketalert"
)

$rgName = "rg-$AppName-$Environment"

Write-Host "Creating resource group..." -ForegroundColor Green
az group create --name $rgName --location $Location

Write-Host "Deploying Bicep template..." -ForegroundColor Green
az deployment group create `
    --resource-group $rgName `
    --template-file "../bicep/main.bicep" `
    --parameters `
        appName=$AppName `
        environmentName=$Environment `
        location=$Location `
        adminPassword="@{adminPassword}" `
        jwtKey="@{jwtKey}" `
        ticketmasterApiKey="@{ticketmasterApiKey}" `
        stripeSecretKey="@{stripeSecretKey}" `
        stripeWebhookSecret="@{stripeWebhookSecret}" `
        smtpHost="@{smtpHost}" `
        smtpUsername="@{smtpUsername}" `
        smtpPassword="@{smtpPassword}"

Write-Host "Deployment complete!" -ForegroundColor Green
