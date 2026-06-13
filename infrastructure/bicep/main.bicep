targetScope = 'subscription'

param location string = 'norwayeast'
param appName string = 'ticketalert'
param environmentName string = 'prod'

@secure()
param adminPassword string

@secure()
param jwtKey string

param jwtIssuer string = 'TicketAlert'
param jwtAudience string = 'TicketAlert'

@secure()
param ticketmasterApiKey string

@secure()
param stripeSecretKey string

@secure()
param stripeWebhookSecret string

param emailFromAddress string = 'noreply@ticketalert.no'

@secure()
param smtpHost string

param smtpPort string = '587'

@secure()
param smtpUsername string

@secure()
param smtpPassword string

param pricingPerWatchNok string = '19'
param corsOrigins string = ''

var rgName = 'rg-${appName}-${environmentName}'
var postgresName = '${appName}-pg-${environmentName}'
var appServicePlanName = '${appName}-plan-${environmentName}'
var appServiceName = '${appName}-api-${environmentName}'

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: rgName
  location: location
}

module postgres 'modules/postgres.bicep' = {
  name: 'postgres-deployment'
  scope: rg
  params: {
    postgresName: postgresName
    location: location
    adminUserName: 'ticketalert_admin'
    adminPassword: adminPassword
  }
}

module appService 'modules/appservice.bicep' = {
  name: 'appservice-deployment'
  scope: rg
  params: {
    appServiceName: appServiceName
    appServicePlanName: appServicePlanName
    location: location
    postgresHost: postgres.outputs.postgresHost
    adminPassword: adminPassword
    jwtKey: jwtKey
    jwtIssuer: jwtIssuer
    jwtAudience: jwtAudience
    ticketmasterApiKey: ticketmasterApiKey
    stripeSecretKey: stripeSecretKey
    stripeWebhookSecret: stripeWebhookSecret
    emailFromAddress: emailFromAddress
    smtpHost: smtpHost
    smtpPort: smtpPort
    smtpUsername: smtpUsername
    smtpPassword: smtpPassword
    pricingPerWatchNok: pricingPerWatchNok
    corsOrigins: corsOrigins != '' ? corsOrigins : 'https://${appServiceName}.azurewebsites.net'
  }
}

output apiUrl string = 'https://${appServiceName}.azurewebsites.net'
output postgresHost string = postgres.outputs.postgresHost
