# Azure DevOps Deployment Guide - News API (Backend Only)

Bu doküman, News API backend'inin Azure Container Apps'e Azure DevOps kullanılarak nasıl deploy edileceğini açıklar.

## 📋 Ön Gereksinimler

### Azure Tarafı
- Azure aboneliği
- Azure CLI yüklü (yerel test için)
- Resource Group oluşturma yetkisi

### Azure DevOps Tarafı
- Azure DevOps organizasyonu ve projesi
- Service Connection (Azure Resource Manager)
- Variable Groups oluşturma yetkisi

### Mevcut Servisler
- **MongoDB sunucusu** (erişilebilir ve çalışır durumda)
- **MinIO sunucusu** (erişilebilir ve çalışır durumda)
- MinIO'da `news-images` bucket'ı oluşturulmuş olmalı

## 🚀 Adım Adım Kurulum

### 1. Azure Container Registry Oluşturma

```bash
# Azure'a giriş yapın
az login

# Resource Group oluşturun
az group create \
  --name newsapi-infrastructure-rg \
  --location eastus

# Azure Container Registry oluşturun
az acr create \
  --resource-group newsapi-infrastructure-rg \
  --name newsapidevacr \
  --sku Basic \
  --admin-enabled true

# ACR bilgilerini kaydedin
az acr credential show --name newsapidevacr
```

**ÖNEMLİ:** ACR adını `azure-pipelines.yml` dosyasındaki `acrName` değişkenine yazın.

### 2. Azure DevOps Service Connection Oluşturma

1. Azure DevOps projenize gidin
2. **Project Settings** > **Service connections** > **New service connection**
3. **Azure Resource Manager** seçin
4. **Service principal (automatic)** seçin
5. Ayarlar:
   - **Subscription**: Azure aboneliğiniz
   - **Resource group**: `newsapi-infrastructure-rg` (veya tümü)
   - **Service connection name**: `Azure-ServiceConnection`
   - ✅ **Grant access permission to all pipelines** işaretleyin

**ÖNEMLİ:** Service connection adını `azure-pipelines.yml` dosyasındaki `azureSubscription` değişkenine yazın.

### 3. Variable Groups Oluşturma

Azure DevOps'ta **Pipelines** > **Library** > **+ Variable group** bölümünden iki adet variable group oluşturun:

#### A) Development Secrets: `newsapi-dev-secrets`

| Variable Name | Value | Type | Örnek |
|---------------|-------|------|-------|
| `JWT_SECRET_KEY` | JWT secret anahtarınız (min 32 karakter) | Secret | `your-super-secret-jwt-key-min-32` |
| `MONGO_CONNECTION_STRING` | MongoDB connection string | Secret | `mongodb://admin:password@your-mongo-server:27017/NewsDb?authSource=admin` |
| `MONGO_DATABASE_NAME` | Database adı | Normal | `NewsDb` |
| `MINIO_ENDPOINT` | MinIO endpoint (port dahil) | Normal | `minio.yourdomain.com:9000` |
| `MINIO_ACCESS_KEY` | MinIO access key | Secret | `minioadmin` |
| `MINIO_SECRET_KEY` | MinIO secret key | Secret | `minioadmin123` |
| `MINIO_BUCKET_NAME` | Bucket adı | Normal | `news-images` |

#### B) Production Secrets: `newsapi-prod-secrets`

Aynı değişkenler + ek olarak:

| Variable Name | Value | Type | Açıklama |
|---------------|-------|------|----------|
| `JWT_ISSUER` | Production issuer URL | Normal | `https://api.yourdomain.com` |
| `JWT_AUDIENCE` | Production audience URL | Normal | `https://yourdomain.com` |

**MongoDB Connection String Örnekleri:**

```bash
# Authentication Database ile
mongodb://username:password@host:27017/NewsDb?authSource=admin

# Replica Set ile
mongodb://username:password@host1:27017,host2:27017/NewsDb?replicaSet=rs0&authSource=admin

# MongoDB Atlas
mongodb+srv://username:password@cluster.mongodb.net/NewsDb?retryWrites=true&w=majority

# SSL/TLS ile
mongodb://username:password@host:27017/NewsDb?ssl=true&authSource=admin
```

### 4. Pipeline Environments Oluşturma

1. **Pipelines** > **Environments** > **New environment**
2. İki environment oluşturun:
   - `development` (dev deploy için)
   - `production` (prod deploy için, approval ekleyebilirsiniz)

**Production için Approval (Opsiyonel):**
- Environment `production` > ⚙️ > **Approvals and checks**
- **Approvals** ekleyin
- Onaylayacak kişileri seçin

### 5. Pipeline Dosyasını Yapılandırma

`azure-pipelines.yml` dosyasında şu değerleri güncelleyin:

```yaml
variables:
  azureSubscription: 'Azure-ServiceConnection'  # Adım 2'de oluşturduğunuz service connection adı
  acrName: 'newsapidevacr'   # Adım 1'de oluşturduğunuz ACR adı
```

### 6. Pipeline'ı Azure DevOps'a Aktarma

#### Yöntem 1: YAML Pipeline Oluşturma (Önerilen)

1. Azure DevOps projenizde **Pipelines** > **New pipeline**
2. **Azure Repos Git** seçin (veya GitHub if using GitHub)
3. Repository'nizi seçin
4. **Existing Azure Pipelines YAML file** seçin
5. Path: `/azure-pipelines.yml` seçin
6. **Continue** > **Run**

#### Yöntem 2: Mevcut Pipeline'ı Güncelleme

1. Mevcut pipeline'ınızı açın
2. **Edit** butonuna tıklayın
3. YAML içeriğini güncelleyin
4. **Save and run**

## 🔄 Kullanım

### Development'a Deploy (Otomatik)

```bash
# develop branch'ine push yapın
git checkout develop
git add .
git commit -m "Deploy to dev"
git push origin develop
```

Pipeline otomatik olarak:
1. Docker image build eder
2. ACR'ye push eder
3. Dev environment'a deploy eder
4. Health check yapar

### Production'a Deploy (Otomatik)

```bash
# master branch'ine merge edin
git checkout master
git merge develop
git push origin master
```

Pipeline otomatik olarak:
1. Docker image build eder
2. ACR'ye push eder
3. Approval bekler (eğer ayarladıysanız)
4. Production'a deploy eder
5. Health check yapar

### Manuel Trigger

1. **Pipelines** > Pipeline'ınızı seçin
2. **Run pipeline**
3. Branch seçin (`master` veya `develop`)
4. **Run**

## 📊 Pipeline Aşamaları

### 1. Build Stage
- Docker image build edilir
- Azure Container Registry'ye push edilir
- Bicep template'leri artifact olarak yayınlanır

### 2. Deploy Dev Stage (develop branch)
- Resource Group oluşturulur
- Azure Container Apps infrastructure deploy edilir
- Container app güncellenir
- Health check çalıştırılır

### 3. Deploy Prod Stage (master branch)
- Resource Group oluşturulur
- Production infrastructure deploy edilir
- Approval beklenir (eğer varsa)
- Container app güncellenir
- Health check çalıştırılır

## 🔍 Troubleshooting

### Build Hatası: "ACR not found"

```bash
# ACR'nin mevcut olduğunu kontrol edin
az acr show --name newsapidevacr

# Eğer yoksa oluşturun
az acr create --resource-group newsapi-infrastructure-rg --name newsapidevacr --sku Basic
```

### Deploy Hatası: "Service connection not found"

1. Service connection adını kontrol edin
2. Service connection'ın tüm pipeline'lara erişim yetkisi olduğunu doğrulayın
3. Service principal'ın subscription'da Contributor rolü olduğunu kontrol edin

### Deploy Hatası: "Secret not found"

1. Variable group adlarını kontrol edin (`newsapi-dev-secrets`, `newsapi-prod-secrets`)
2. Variable group'ların pipeline'a bağlı olduğunu doğrulayın
3. Secret değişkenlerin "lock" ikonuyla işaretlendiğini kontrol edin

### Health Check Başarısız

```bash
# Container logs kontrol edin
az containerapp logs show \
  --name newsapi-dev-app \
  --resource-group newsapi-rg-dev \
--follow

# MongoDB bağlantısını test edin
# Container içinde:
curl http://localhost:8080/health
```

### MongoDB Bağlantı Hatası

Connection string formatını kontrol edin:
```
mongodb://username:password@host:port/database?authSource=admin
```

Firewall kurallarını kontrol edin:
```bash
# Azure Container App outbound IP'lerini MongoDB'ye ekleyin
az containerapp show \
  --name newsapi-dev-app \
  --resource-group newsapi-rg-dev \
  --query properties.outboundIpAddresses
```

### MinIO Bağlantı Hatası

1. Endpoint formatını kontrol edin: `minio.yourdomain.com:9000` (http:// olmadan)
2. Bucket'ın mevcut olduğunu doğrulayın
3. Access key ve secret key'i kontrol edin
4. MinIO'nun public erişime açık olduğunu veya firewall kurallarının doğru olduğunu kontrol edin

## 📈 Monitoring

### Azure Portal'da Monitoring

1. **Azure Portal** > **Resource Groups** > `newsapi-rg-dev`
2. Container App'i seçin
3. **Monitoring** > **Metrics** veya **Logs**

### Application Insights

```bash
# App Insights connection string'i alın
az monitor app-insights component show \
  --app newsapi-dev-insights \
  --resource-group newsapi-rg-dev \
  --query connectionString
```

### Log Analytics

```bash
# Son 100 logu görüntüleyin
az containerapp logs show \
  --name newsapi-dev-app \
  --resource-group newsapi-rg-dev \
  --tail 100
```

## 🔒 Güvenlik Best Practices

### 1. Azure Key Vault Kullanımı (Önerilen)

Production için tüm secret'ları Azure Key Vault'ta saklayın:

```bash
# Key Vault oluşturun
az keyvault create \
  --name newsapi-vault-prod \
  --resource-group newsapi-infrastructure-rg \
  --location eastus

# Secret'ları ekleyin
az keyvault secret set --vault-name newsapi-vault-prod --name jwt-secret --value "your-secret"
az keyvault secret set --vault-name newsapi-vault-prod --name mongo-connection --value "mongodb://..."
```

Pipeline'da Key Vault task kullanın:
```yaml
- task: AzureKeyVault@2
  inputs:
    azureSubscription: $(azureSubscription)
    KeyVaultName: 'newsapi-vault-prod'
    SecretsFilter: '*'
```

### 2. Managed Identity

Container App'e Managed Identity ekleyin ve Key Vault erişimi verin:

```bash
# Managed Identity enable edin
az containerapp identity assign \
  --name newsapi-prod-app \
  --resource-group newsapi-rg-prod \
  --system-assigned

# Key Vault access policy ekleyin
az keyvault set-policy \
--name newsapi-vault-prod \
  --object-id <managed-identity-principal-id> \
  --secret-permissions get list
```

### 3. Network Security

MongoDB ve MinIO için:
- IP whitelisting yapın
- VNet integration kullanın (Azure Container Apps VNet support)
- SSL/TLS kullanın

## 💰 Maliyet Optimizasyonu

### Development Environment

```yaml
# Scale down during off-hours
minReplicas: 0  # Dev environment için
maxReplicas: 1
```

### Scheduled Scaling (Opsiyonel)

Azure Automation kullanarak off-hours'da container'ı durdurun:

```bash
# Container app'i durdur
az containerapp update \
  --name newsapi-dev-app \
  --resource-group newsapi-rg-dev \
  --min-replicas 0 \
  --max-replicas 0
```

## 📚 Ek Kaynaklar

- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [Azure DevOps Pipelines](https://learn.microsoft.com/azure/devops/pipelines/)
- [Azure Bicep Documentation](https://learn.microsoft.com/azure/azure-resource-manager/bicep/)
- [MongoDB Connection Strings](https://www.mongodb.com/docs/manual/reference/connection-string/)
- [MinIO Client Documentation](https://min.io/docs/minio/linux/reference/minio-mc.html)

## 🆘 Destek

Sorun yaşıyorsanız:
1. Pipeline loglarını kontrol edin
2. Azure Portal'da resource'ları kontrol edin
3. Container app loglarını inceleyin
4. Bu dokümandaki troubleshooting bölümünü kontrol edin

Pipeline log'unda hata mesajlarını arayın:
- `error` keyword'ü
- HTTP status code'lar (400, 401, 403, 404, 500)
- Connection timeout mesajları
