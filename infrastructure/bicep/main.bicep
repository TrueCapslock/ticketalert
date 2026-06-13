param location string = 'norwayeast'
param appName string = 'ticketalert'
param environmentName string = 'prod'

var postgresName = '${appName}-pg-${environmentName}'
var appServicePlanName = '${appName}-plan-${environmentName}'
var appServiceName = '${appName}-api-${environmentName}'

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: 'rg-${appName}-${environmentName}'
  location: location
}

resource postgres 'Microsoft.DBforPostgreSQL/flexibleServers@2023-06-01-preview' = {
  name: postgresName
  location: location
  resourceGroup: rg.name
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
  properties: {
    administratorLogin: 'ticketalert_admin'
    administratorLoginPassword: '@{adminPassword}'
    version: '16'
    storage: {
      storageSizeGB: 32
    }
    highAvailability: {
      mode: 'Disabled'
    }
  }
}

resource postgresFirewall 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-06-01-preview' = {
  name: '${postgresName}/AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource postgresDatabase 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2023-06-01-preview' = {
  name: '${postgresName}/ticketalert'
}

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  resourceGroup: rg.name
  sku: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 1
  }
}

resource appService 'Microsoft.Web/sites@2023-01-01' = {
  name: appServiceName
  location: location
  resourceGroup: rg.name
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      netFrameworkVersion: 'v9.0'
      appSettings: [
        { name: 'ConnectionStrings__DefaultConnection', value: 'Host=${postgres.properties.fullyQualifiedDomainName};Database=ticketalert;Username=ticketalert_admin;Password=${adminPassword}' }
        { name: 'Jwt__Key', value: '@{jwtKey}' }
        { name: 'Jwt__Issuer', value: 'TicketAlert' }
        { name: 'Jwt__Audience', value: 'TicketAlert' }
        { name: 'Ticketmaster__ApiKey', value: '@{ticketmasterApiKey}' }
        { name: 'Stripe__SecretKey', value: '@{stripeSecretKey}' }
        { name: 'Stripe__WebhookSecret', value: '@{stripeWebhookSecret}' }
        { name: 'Email__FromAddress', value: 'noreply@ticketalert.no' }
        { name: 'Email__SmtpHost', value: '@{smtpHost}' }
        { name: 'Email__SmtpPort', value: '587' }
        { name: 'Email__Username', value: '@{smtpUsername}' }
        { name: 'Email__Password', value: '@{smtpPassword}' }
        { name: 'Pricing__PerWatchNok', value: '19' }
        { name: 'Cors__Origins', value: 'https://${appServiceName}.azurewebsites.net' }
        { name: 'ASPNETCORE_ENVIRONMENT', value: 'Production' }
      ]
    }
  }
}

output apiUrl string = 'https://${appServiceName}.azurewebsites.net'
output postgresHost string = postgres.properties.fullyQualifiedDomainName
