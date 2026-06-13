param appServiceName string
param appServicePlanName string
param location string
param postgresHost string
param adminPassword string
param jwtKey string
param jwtIssuer string
param jwtAudience string
param ticketmasterApiKey string
param stripeSecretKey string
param stripeWebhookSecret string
param emailFromAddress string
param smtpHost string
param smtpPort string
param smtpUsername string
param smtpPassword string
param pricingPerWatchNok string
param corsOrigins string

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
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
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      netFrameworkVersion: 'v9.0'
      appSettings: [
        { name: 'ConnectionStrings__DefaultConnection', value: 'Host=${postgresHost};Database=ticketalert;Username=ticketalert_admin;Password=${adminPassword}' }
        { name: 'Jwt__Key', value: jwtKey }
        { name: 'Jwt__Issuer', value: jwtIssuer }
        { name: 'Jwt__Audience', value: jwtAudience }
        { name: 'Ticketmaster__ApiKey', value: ticketmasterApiKey }
        { name: 'Stripe__SecretKey', value: stripeSecretKey }
        { name: 'Stripe__WebhookSecret', value: stripeWebhookSecret }
        { name: 'Email__FromAddress', value: emailFromAddress }
        { name: 'Email__SmtpHost', value: smtpHost }
        { name: 'Email__SmtpPort', value: smtpPort }
        { name: 'Email__Username', value: smtpUsername }
        { name: 'Email__Password', value: smtpPassword }
        { name: 'Pricing__PerWatchNok', value: pricingPerWatchNok }
        { name: 'Cors__Origins', value: corsOrigins }
        { name: 'ASPNETCORE_ENVIRONMENT', value: 'Production' }
      ]
    }
  }
}
