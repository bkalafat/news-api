# Azure Deployment - News API (Backend Only)

News API backend'ini Azure Container Apps'e deploy etmek için gerekli tüm dosyalar.

## 📁 Dosya Yapısı

```
azure/
├── bicep/
│   ├── main.bicep       # Ana infrastructure template (sadece Container App)
│   ├── parameters.dev.json      # Development parametreleri
│   └── parameters.prod.json     # Production parametreleri
├── scripts/
│   ├── deploy.sh                  # Linux/Mac deployment script
│   └── deploy.ps1             # Windows deployment script
├── AZURE_DEVOPS_GUIDE.md          # Azure DevOps pipeline kurulum rehberi
└── DEPLOYMENT_GUIDE.md            # Manuel deployment rehberi
```

## 🎯 Özellikler

✅ **Sadece Backend API Deploy Edilir**
- Azure Container Apps ile otomatik ölçeklendirme
- Application Insights ile monitoring
- Log Analytics ile merkezi log yönetimi
- Azure Container Registry ile özel Docker image hosting

✅ **Mevcut Servislerinizi Kullanır**
- Kendi MongoDB sunucunuz
- Kendi MinIO sunucunuz
- Cosmos DB veya Azure Blob Storage gerektirmez

✅ **İki Deployment Yöntemi**
- **Azure DevOps Pipeline** (Otomatik CI/CD)
- **Manuel Deployment** (Script tabanlı)

## 🚀 Hızlı Başlangıç

### Azure DevOps ile (Önerilen)

1. **[Azure DevOps Setup Guide](./AZURE_DEVOPS_GUIDE.md)** dokümanını takip edin
2. Service connection ve variable groups oluşturun
3. Pipeline'ı çalıştırın
4. ✅ Otomatik deploy!

### Manuel Deployment ile

#### Linux/Mac:
```bash
cd azure/scripts
chmod +x deploy.sh
./deploy.sh
```

#### Windows (PowerShell):
```powershell
cd azure\scripts
.\deploy.ps1
```

Script size şunları soracak:
- Environment (dev/staging/prod)
- Azure region
- Container Registry adı
- JWT secret key
- **MongoDB connection string**
- **MinIO endpoint ve credentials**

## 📋 Ön Gereksinimler

### Azure
- ✅ Azure aboneliği ([Ücretsiz hesap](https://azure.microsoft.com/free/))
- ✅ Azure CLI yüklü ([İndir](https://learn.microsoft.com/cli/azure/install-azure-cli))
- ✅ Docker yüklü ve çalışır durumda

### Mevcut Servisler
- ✅ **MongoDB sunucusu** (erişilebilir durumda)
  - Connection string hazır
  - Database: `NewsDb` oluşturulmuş
  - User credentials hazır
  
- ✅ **MinIO sunucusu** (erişilebilir durumda)
  - Endpoint URL
  - Access key ve secret key
  - `news-images` bucket'ı oluşturulmuş

### Azure DevOps (sadece pipeline için)
- ✅ Azure DevOps organizasyonu
- ✅ Proje oluşturulmuş
- ✅ Repo'ya erişim

## 🏗️ Azure'da Oluşturulacak Kaynaklar

| Kaynak | Amaç | Tahmini Maliyet/Ay |
|--------|------|-------------------|
| **Azure Container Apps** | API hosting | $5-50 |
| **Azure Container Registry** | Docker image storage | $5 |
| **Log Analytics Workspace** | Log toplama | $5-20 |
| **Application Insights** | Monitoring ve telemetri | $0-10 |
| **TOPLAM** | | **$15-85** |

> **Not:** Cosmos DB ve Azure Blob Storage KULLANILMAZ (kendi MongoDB ve MinIO'nuzu kullanırsınız).

## 🔧 Konfigürasyon

### MongoDB Connection String Örnekleri

```bash
# Basit authentication
mongodb://admin:password@your-server.com:27017/NewsDb?authSource=admin

# Replica set
mongodb://user:pass@host1:27017,host2:27017/NewsDb?replicaSet=rs0&authSource=admin

# MongoDB Atlas
mongodb+srv://username:password@cluster.mongodb.net/NewsDb?retryWrites=true

# SSL ile
mongodb://user:pass@host:27017/NewsDb?ssl=true&authSource=admin

# Docker'daki local MongoDB (geliştirme için)
mongodb://admin:password123@host.docker.internal:27017/NewsDb?authSource=admin
```

### MinIO Konfigürasyonu

```bash
# Endpoint formatı (http:// OLMADAN)
minio.yourdomain.com:9000

# Veya IP adresi
192.168.1.100:9000

# Docker'daki local MinIO (geliştirme için)
host.docker.internal:9000
```

**MinIO Bucket Hazırlığı:**
```bash
# mc (MinIO Client) ile bucket oluşturma
mc alias set myminio http://your-minio:9000 accesskey secretkey
mc mb myminio/news-images
mc policy set download myminio/news-images  # Public read için
```

## 🌐 Deployment Sonrası

### Uygulama URL'leri

```bash
# Development
https://newsapi-dev-app.{region}.azurecontainerapps.io

# Production
https://newsapi-prod-app.{region}.azurecontainerapps.io
```

### API Test

```bash
# Health check
curl https://newsapi-dev-app.{region}.azurecontainerapps.io/health

# Swagger UI
https://newsapi-dev-app.{region}.azurecontainerapps.io/swagger
```

### Login ve Test

1. Swagger UI'ı açın
2. `/api/Auth/login` endpoint'ini kullanın:
   ```json
   {
     "username": "admin",
     "password": "admin123"
   }
   ```
3. Token'ı alın
4. "Authorize" butonuna tıklayın
5. `Bearer {token}` girin
6. API endpoint'lerini test edin

## 📊 Monitoring

### Azure Portal

1. **Resource Group** > `newsapi-rg-{env}` açın
2. **Container App** > Metrics veya Logs
3. **Application Insights** > Açın

### CLI ile Log İzleme

```bash
# Live logs
az containerapp logs show \
  --name newsapi-dev-app \
  --resource-group newsapi-rg-dev \
  --follow

# Son 100 log
az containerapp logs show \
  --name newsapi-dev-app \
  --resource-group newsapi-rg-dev \
  --tail 100
```

### Application Insights Queries

```kusto
// Son 1 saatteki hatalar
traces
| where timestamp > ago(1h)
| where severityLevel >= 3
| order by timestamp desc

// API response times
requests
| where timestamp > ago(1h)
| summarize avg(duration), max(duration) by name
```

## 🔒 Güvenlik

### Network Güvenliği

**MongoDB Firewall Kuralları:**
```bash
# Container App outbound IP'lerini alın
az containerapp show \
  --name newsapi-dev-app \
  --resource-group newsapi-rg-dev \
  --query properties.outboundIpAddresses

# Bu IP'leri MongoDB firewall'una ekleyin
```

**MinIO Access Policy:**
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
 "Effect": "Allow",
      "Principal": {"AWS": ["*"]},
      "Action": ["s3:GetObject"],
 "Resource": ["arn:aws:s3:::news-images/*"]
    }
  ]
}
```

### CORS Ayarları

Backend'de CORS'u frontend domain'iniz için açın:

```csharp
// backend/Presentation/Extensions/ServiceCollectionExtensions.cs
builder.WithOrigins(
    "https://your-frontend.com",
    "https://newsapi-dev-app.eastus.azurecontainerapps.io"
)
```

## 🔄 Güncelleme ve Rollback

### Yeni Version Deploy

```bash
# Development
git push origin develop

# Production
git push origin master
```

### Rollback (Azure CLI)

```bash
# Önceki revision'ı bulun
az containerapp revision list \
  --name newsapi-prod-app \
  --resource-group newsapi-rg-prod \
  --query "[].name"

# Rollback
az containerapp revision activate \
  --name newsapi-prod-app \
  --resource-group newsapi-rg-prod \
  --revision {revision-name}
```

## 🗑️ Temizlik

### Development Environment Silme

```bash
az group delete --name newsapi-rg-dev --yes --no-wait
```

### Sadece Container App'i Durdurma (data kalır)

```bash
az containerapp update \
  --name newsapi-dev-app \
  --resource-group newsapi-rg-dev \
  --min-replicas 0 \
  --max-replicas 0
```

## 📚 Dokümantasyon

- **[Azure DevOps Pipeline Guide](./AZURE_DEVOPS_GUIDE.md)** - Otomatik CI/CD kurulumu
- **[Manual Deployment Guide](./DEPLOYMENT_GUIDE.md)** - Manuel deployment adımları
- **[Bicep Template](./bicep/main.bicep)** - Infrastructure as Code

## 🆘 Troubleshooting

### Sık Karşılaşılan Sorunlar

**1. MongoDB bağlanamıyor**
- Connection string formatını kontrol edin
- MongoDB sunucusunun erişilebilir olduğunu doğrulayın
- Firewall kurallarını kontrol edin
- Authentication credentials'ı doğrulayın

**2. MinIO'ya upload yapamıyor**
- Endpoint formatını kontrol edin (http:// olmadan)
- Access key/secret key'i doğrulayın
- Bucket'ın mevcut olduğunu kontrol edin
- MinIO'nun erişilebilir olduğunu test edin

**3. Container başlamıyor**
- Container logs'u kontrol edin: `az containerapp logs show`
- Environment variable'ları kontrol edin
- Docker image'ın push edildiğini doğrulayın

**4. Health check başarısız**
- `/health` endpoint'inin çalıştığını local'de test edin
- Container'ın 8080 portunda dinlediğini doğrulayın
- Startup time'ı artırın (initialDelaySeconds)

### Destek

Sorun yaşarsanız:
1. Container logs'u kontrol edin
2. Azure Portal'da resource health'i inceleyin
3. Troubleshooting guide'ı gözden geçirin
4. GitHub issue açın

## 📞 İletişim

- **GitHub Issues**: Sorun bildirmek için
- **Azure Support**: Production kritik sorunlar için

---

**Last Updated:** January 2025  
**Version:** 1.0  
**Compatibility:** .NET 10, Azure Container Apps
