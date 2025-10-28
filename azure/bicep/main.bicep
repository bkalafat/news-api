// ============================================
// News API - Azure Infrastructure (API Only)
// ============================================
// This Bicep template deploys only the News API Container App to Azure
// External MongoDB and MinIO services will be used

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

@description('External MongoDB connection string')
@secure()
param mongoConnectionString string

@description('MongoDB database name')
param mongoDatabaseName string = 'NewsDb'

@description('MinIO endpoint URL (e.g., https://minio.yourdomain.com)')
param minioEndpoint string

@description('MinIO access key')
@secure()
param minioAccessKey string

@description('MinIO secret key')
@secure()
param minioSecretKey string

@description('MinIO bucket name for images')
param minioBucketName string = 'news-images'

@description('JWT Issuer URL')
param jwtIssuer string = ''

@description('JWT Audience URL')
param jwtAudience string = ''

@description('Minimum replica count for Container App')
@minValue(0)
@maxValue(10)
param minReplicas int = 1

@description('Maximum replica count for Container App')
@minValue(1)
@maxValue(30)
param maxReplicas int = 5

@description('Container CPU cores')
param containerCpu string = '0.5'

@description('Container memory')
param containerMemory string = '1Gi'

// ============================================
// Variables
// ============================================
var resourcePrefix = '${appName}-${environmentName}'
var containerAppEnvName = '${resourcePrefix}-env'
var containerAppName = '${resourcePrefix}-app'
var logAnalyticsWorkspaceName = '${resourcePrefix}-logs'
var appInsightsName = '${resourcePrefix}-insights'
var containerRegistryName = replace('${resourcePrefix}acr', '-', '')

// Dynamic JWT values
var jwtIssuerValue = empty(jwtIssuer) ? 'https://${containerAppName}.${containerAppEnv.properties.defaultDomain}' : jwtIssuer
var jwtAudienceValue = empty(jwtAudience) ? 'https://${containerAppName}.${containerAppEnv.properties.defaultDomain}' : jwtAudience

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
// Container Registry
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
        transport: 'auto'
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
     value: mongoConnectionString
     }
        {
       name: 'minio-access-key'
          value: minioAccessKey
        }
    {
        name: 'minio-secret-key'
      value: minioSecretKey
  }
      ]
  }
    template: {
    containers: [
        {
          name: containerAppName
          image: containerImage
 resources: {
  cpu: json(containerCpu)
            memory: containerMemory
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
            // MongoDB Configuration
  {
              name: 'MongoDB__ConnectionString'
            secretRef: 'mongo-connection'
            }
       {
         name: 'MongoDB__DatabaseName'
       value: mongoDatabaseName
 }
            // JWT Configuration
        {
              name: 'JWT__SecretKey'
        secretRef: 'jwt-secret'
         }
     {
       name: 'JWT__Issuer'
     value: jwtIssuerValue
        }
        {
        name: 'JWT__Audience'
  value: jwtAudienceValue
        }
            {
       name: 'JWT__ExpiryMinutes'
        value: '60'
       }
            // MinIO Configuration
            {
      name: 'MinIO__Endpoint'
              value: minioEndpoint
          }
  {
         name: 'MinIO__AccessKey'
   secretRef: 'minio-access-key'
    }
            {
  name: 'MinIO__SecretKey'
    secretRef: 'minio-secret-key'
        }
            {
       name: 'MinIO__BucketName'
        value: minioBucketName
          }
{
 name: 'MinIO__UseSSL'
   value: 'true'
   }
    // Application Insights
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
       timeoutSeconds: 10
              failureThreshold: 3
            }
            {
      type: 'Readiness'
     httpGet: {
path: '/health'
      port: 8080
       }
       initialDelaySeconds: 10
    periodSeconds: 10
  timeoutSeconds: 5
     failureThreshold: 3
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
output containerAppName string = containerApp.name
output containerRegistryLoginServer string = containerRegistry.properties.loginServer
output containerRegistryName string = containerRegistry.name
output appInsightsInstrumentationKey string = appInsights.properties.InstrumentationKey
output appInsightsConnectionString string = appInsights.properties.ConnectionString
output resourceGroupName string = resourceGroup().name
output logAnalyticsWorkspaceId string = logAnalytics.id
