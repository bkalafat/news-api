// ============================================
// News API - Azure Infrastructure
// ============================================
// This Bicep template deploys the complete News API infrastructure to Azure
// including Container App, Cosmos DB (MongoDB API), and Blob Storage

targetScope = 'resourceGroup'

// ============================================
// Parameters
// ============================================
@description('Environment name (dev, staging, prod)')
@allowed([
  'dev'
  'staging'
  'prod'
])
param environmentName string = 'dev'

@description('Location for all resources')
param location string = resourceGroup().location

@description('Name of the application')
param appName string = 'newsapi'

@description('Container image and tag')
param containerImage string = 'newsapi:latest'

@description('JWT Secret Key for authentication')
@secure()
param jwtSecretKey string

@description('MongoDB admin username')
param mongoAdminUsername string = 'newsapiadmin'

@description('MongoDB admin password')
@secure()
param mongoAdminPassword string

@description('Minimum replica count for Container App')
@minValue(0)
@maxValue(10)
param minReplicas int = 1

@description('Maximum replica count for Container App')
@minValue(1)
@maxValue(30)
param maxReplicas int = 5

// ============================================
// Variables
// ============================================
var resourcePrefix = '${appName}-${environmentName}'
var containerAppEnvName = '${resourcePrefix}-env'
var containerAppName = '${resourcePrefix}-app'
var cosmosDbAccountName = '${resourcePrefix}-cosmos'
var cosmosDbName = 'NewsDb'
var storageAccountName = replace('${resourcePrefix}storage', '-', '')
var logAnalyticsWorkspaceName = '${resourcePrefix}-logs'
var appInsightsName = '${resourcePrefix}-insights'
var containerRegistryName = replace('${resourcePrefix}acr', '-', '')

// ============================================
// Log Analytics Workspace
// ============================================
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// ============================================
// Application Insights
// ============================================
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
    RetentionInDays: 30
  }
}

// ============================================
// Azure Cosmos DB (MongoDB API)
// ============================================
resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2023-11-15' = {
  name: cosmosDbAccountName
  location: location
  kind: 'MongoDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
    defaultConsistencyLevel: 'Session'
    }
    locations: [
    {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    apiProperties: {
  serverVersion: '7.0'
    }
    capabilities: [
      {
  name: 'EnableMongo'
   }
  ]
    enableAutomaticFailover: false
    enableMultipleWriteLocations: false
  }
}

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts/mongodbDatabases@2023-11-15' = {
  parent: cosmosDbAccount
  name: cosmosDbName
  properties: {
    resource: {
      id: cosmosDbName
    }
    options: {
      throughput: 400 // Minimum for shared throughput
  }
  }
}

resource newsCollection 'Microsoft.DocumentDB/databaseAccounts/mongodbDatabases/collections@2023-11-15' = {
  parent: cosmosDb
  name: 'News'
  properties: {
    resource: {
      id: 'News'
      shardKey: {
        _id: 'Hash'
    }
      indexes: [
     {
          key: {
       keys: [
   '_id'
  ]
          }
  }
      {
          key: {
            keys: [
       'CreatedDate'
            ]
}
     }
      ]
    }
  }
}

// ============================================
// Storage Account (replaces MinIO)
// ============================================
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: true
    minimumTlsVersion: 'TLS1_2'
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
}

resource newsImagesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: 'news-images'
  properties: {
    publicAccess: 'Blob'
  }
}

// ============================================
// Container Registry (optional, for private images)
// ============================================
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: containerRegistryName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

// ============================================
// Container Apps Environment
// ============================================
resource containerAppEnv 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: containerAppEnvName
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

// ============================================
// Container App
// ============================================
resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: containerAppEnv.id
    configuration: {
    ingress: {
        external: true
        targetPort: 8080
        transport: 'http'
        allowInsecure: false
      }
      registries: [
        {
          server: containerRegistry.properties.loginServer
username: containerRegistry.listCredentials().username
  passwordSecretRef: 'acr-password'
      }
      ]
      secrets: [
        {
          name: 'acr-password'
          value: containerRegistry.listCredentials().passwords[0].value
        }
        {
          name: 'jwt-secret'
    value: jwtSecretKey
        }
 {
          name: 'mongo-connection'
          value: cosmosDbAccount.listConnectionStrings().connectionStrings[0].connectionString
        }
        {
          name: 'storage-connection'
     value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
        }
      ]
    }
    template: {
      containers: [
        {
          name: containerAppName
          image: containerImage
      resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
  env: [
   {
   name: 'ASPNETCORE_ENVIRONMENT'
       value: environmentName == 'prod' ? 'Production' : 'Development'
    }
          {
      name: 'ASPNETCORE_URLS'
      value: 'http://+:8080'
     }
  {
     name: 'MongoDB__ConnectionString'
              secretRef: 'mongo-connection'
          }
  {
     name: 'MongoDB__DatabaseName'
     value: cosmosDbName
  }
     {
              name: 'JWT__SecretKey'
   secretRef: 'jwt-secret'
         }
    {
         name: 'JWT__Issuer'
      value: 'https://${containerAppName}.${containerAppEnv.properties.defaultDomain}'
            }
{
        name: 'JWT__Audience'
              value: 'https://${containerAppName}.${containerAppEnv.properties.defaultDomain}'
         }
       {
   name: 'JWT__ExpiryMinutes'
        value: '60'
         }
        {
              name: 'Storage__ConnectionString'
 secretRef: 'storage-connection'
        }
      {
           name: 'Storage__ContainerName'
              value: 'news-images'
    }
    {
     name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: appInsights.properties.ConnectionString
        }
          ]
   probes: [
            {
     type: 'Liveness'
          httpGet: {
    path: '/health'
         port: 8080
              }
     initialDelaySeconds: 30
          periodSeconds: 30
        }
 {
    type: 'Readiness'
  httpGet: {
    path: '/health'
          port: 8080
     }
       initialDelaySeconds: 10
   periodSeconds: 10
            }
    ]
      }
  ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
        rules: [
          {
       name: 'http-scaling'
       http: {
  metadata: {
                concurrentRequests: '50'
    }
     }
       }
    ]
      }
    }
  }
}

// ============================================
// Outputs
// ============================================
output containerAppUrl string = 'https://${containerApp.properties.configuration.ingress.fqdn}'
output cosmosDbConnectionString string = cosmosDbAccount.listConnectionStrings().connectionStrings[0].connectionString
output storageAccountName string = storageAccount.name
output containerRegistryLoginServer string = containerRegistry.properties.loginServer
output appInsightsInstrumentationKey string = appInsights.properties.InstrumentationKey
output resourceGroupName string = resourceGroup().name
