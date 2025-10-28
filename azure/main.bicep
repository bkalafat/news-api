// Azure Resource Deployment - 100% Free Tier
// Static Web Apps (Frontend) + Container Apps (Backend)

param location string = 'westeurope'
param projectName string = 'newsportal'
param repositoryUrl string = 'https://github.com/bkalafat/newsportal'
param branch string = 'master'

// MongoDB Atlas connection (external, free M0)
@secure()
param mongoDbConnectionString string

// Cloudflare R2 settings (external, free 10GB)
param minioEndpoint string
param minioAccessKey string
@secure()
param minioSecretKey string
param minioUseSSL bool = true

// Unsplash API (external, free)
@secure()
param unsplashAccessKey string

// JWT Secret
@secure()
param jwtSecretKey string

// Static Web App (Frontend) - FREE TIER
resource staticWebApp 'Microsoft.Web/staticSites@2023-01-01' = {
  name: '${projectName}-frontend'
  location: location
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    repositoryUrl: repositoryUrl
    branch: branch
    buildProperties: {
      appLocation: '/frontend'
      apiLocation: ''
      outputLocation: '.next'
      appBuildCommand: 'npm run build'
      apiBuildCommand: ''
    }
    stagingEnvironmentPolicy: 'Enabled'
    allowConfigFileUpdates: true
    provider: 'GitHub'
  }
}

// Container Apps Environment - FREE TIER (2M requests/month)
resource containerAppEnv 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: '${projectName}-env'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
    }
  }
}

// Container App (Backend) - FREE CONSUMPTION TIER
resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: '${projectName}-backend'
  location: location
  properties: {
    managedEnvironmentId: containerAppEnv.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      registries: []
      secrets: [
        {
          name: 'mongodb-connection'
          value: mongoDbConnectionString
        }
        {
          name: 'minio-secret-key'
          value: minioSecretKey
        }
        {
          name: 'unsplash-key'
          value: unsplashAccessKey
        }
        {
          name: 'jwt-secret'
          value: jwtSecretKey
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'newsapi'
          image: 'mcr.microsoft.com/dotnet/samples:aspnetapp' // Temporary, will be replaced by GitHub Actions
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Production'
            }
            {
              name: 'MongoDbSettings__ConnectionString'
              secretRef: 'mongodb-connection'
            }
            {
              name: 'MongoDbSettings__DatabaseName'
              value: 'NewsDb'
            }
            {
              name: 'MongoDbSettings__NewsCollectionName'
              value: 'News'
            }
            {
              name: 'MinioSettings__Endpoint'
              value: minioEndpoint
            }
            {
              name: 'MinioSettings__AccessKey'
              value: minioAccessKey
            }
            {
              name: 'MinioSettings__SecretKey'
              secretRef: 'minio-secret-key'
            }
            {
              name: 'MinioSettings__BucketName'
              value: 'news-images'
            }
            {
              name: 'MinioSettings__UseSSL'
              value: string(minioUseSSL)
            }
            {
              name: 'UnsplashSettings__AccessKey'
              secretRef: 'unsplash-key'
            }
            {
              name: 'JwtSettings__SecretKey'
              secretRef: 'jwt-secret'
            }
            {
              name: 'JwtSettings__Issuer'
              value: 'NewsApi'
            }
            {
              name: 'JwtSettings__Audience'
              value: 'NewsApiUsers'
            }
            {
              name: 'JwtSettings__ExpirationMinutes'
              value: '60'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 0 // Scale to zero when idle (FREE tier benefit)
        maxReplicas: 1
        rules: [
          {
            name: 'http-scaling'
            http: {
              metadata: {
                concurrentRequests: '10'
              }
            }
          }
        ]
      }
    }
  }
}

// Outputs
output staticWebAppUrl string = staticWebApp.properties.defaultHostname
output staticWebAppName string = staticWebApp.name
output backendUrl string = containerApp.properties.configuration.ingress.fqdn
output backendName string = containerApp.name
output resourceGroupName string = resourceGroup().name
