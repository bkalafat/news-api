# Image Upload Strategy for Admin Panel

## Overview
Bu doküman, News API için görsel yükleme stratejisini ve gelecekteki WYSIWYG admin paneli için tasarım kararlarını açıklar.

## 📋 Current Implementation (Phase 1)

### Storage Architecture
- **Storage Provider**: MinIO (S3-compatible object storage)
- **Bucket Name**: `news-images`
- **Public Access**: Enabled for read operations
- **Folder Structure**:
  ```
  news-images/
  ├── technology/
  ├── sports/
  ├── world/
  ├── business/
  ├── science/
  ├── health/
  └── entertainment/
  ```

### Image URL Pattern
```
http://localhost:9000/news-images/{category}/{filename}
```

**Production Pattern** (with CDN):
```
https://cdn.newsapi.com/images/{category}/{filename}
```

## 🎯 Admin Panel Upload Flow (Phase 2 - Future)

### Option 1: Backend-Mediated Upload (RECOMMENDED)

**Workflow:**
1. User selects image in WYSIWYG editor or upload form
2. Frontend sends image to Backend API endpoint: `POST /api/Admin/UploadImage`
3. Backend validates image (size, format, dimensions)
4. Backend uploads to MinIO using `MinioImageStorageService`
5. Backend returns public URL to frontend
6. Frontend inserts URL into article content or sets as featured image

**Advantages:**
- ✅ Centralized validation and security
- ✅ Server-side virus scanning possible
- ✅ Easier to implement rate limiting
- ✅ Can generate thumbnails server-side
- ✅ Logging and audit trail

**Implementation:**
```csharp
[HttpPost("upload-image")]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<ImageUploadResponse>> UploadImage([FromForm] IFormFile file)
{
    // Validate file
    if (!IsValidImage(file))
        return BadRequest("Invalid image file");
    
    // Upload to MinIO
    var metadata = await _minioService.UploadImageAsync(file);
    
    return Ok(new ImageUploadResponse 
    {
        Url = metadata.ImageUrl,
        ThumbnailUrl = metadata.ThumbnailUrl,
        FileName = metadata.FileName
    });
}
```

### Option 2: Direct Frontend Upload

**Workflow:**
1. Frontend requests presigned URL from backend: `GET /api/Admin/GetUploadUrl`
2. Backend generates temporary MinIO presigned URL (expires in 15 mins)
3. Frontend uploads directly to MinIO using presigned URL
4. Frontend notifies backend of successful upload
5. Backend validates and records metadata

**Advantages:**
- ✅ No bandwidth usage on backend
- ✅ Faster uploads (direct to storage)
- ✅ Reduced server load

**Disadvantages:**
- ❌ Complex implementation
- ❌ Harder to validate images
- ❌ Security concerns with client-side uploads

## 📐 Image Processing Requirements

### Validation Rules
```yaml
Allowed Formats: [jpg, jpeg, png, webp]
Max File Size: 5 MB
Max Dimensions: 4000x4000 px
Min Dimensions: 800x600 px
```

### Auto-Generated Variants
1. **Original** - Full resolution (stored as uploaded)
2. **Thumbnail** - 300x200px (for listings)
3. **Medium** - 800x600px (for article previews)
4. **Large** - 1200x800px (for article detail)

### File Naming Convention
```
{category}/{timestamp}-{slug}-{variant}.{ext}

Examples:
technology/20251023-ai-breakthrough-original.jpg
technology/20251023-ai-breakthrough-thumbnail.jpg
technology/20251023-ai-breakthrough-medium.jpg
```

## 🔒 Security Considerations

### Upload Endpoint Security
- **Authentication**: JWT Bearer token required
- **Authorization**: Admin role required
- **Rate Limiting**: 10 uploads per minute per user
- **File Validation**: Magic number check (not just extension)
- **Virus Scanning**: ClamAV integration (future)

### MinIO Security
- **Access Control**: Public read, authenticated write
- **Bucket Policy**: Restrict upload to backend service account only
- **CORS**: Whitelist frontend domain
- **Lifecycle Policy**: Auto-delete orphaned images after 30 days

## 📝 WYSIWYG Editor Integration

### Recommended Editor: TinyMCE or Quill.js

**TinyMCE Configuration:**
```javascript
tinymce.init({
  selector: '#article-content',
  plugins: 'image imagetools media link',
  images_upload_handler: async (blobInfo, success, failure) => {
    const formData = new FormData();
    formData.append('file', blobInfo.blob(), blobInfo.filename());
    
    const response = await fetch('/api/Admin/UploadImage', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData
    });
    
    if (response.ok) {
      const data = await response.json();
      success(data.url);
    } else {
      failure('Image upload failed');
    }
  }
});
```

## 🗄️ Database Schema Updates

### ImageMetadata Table (Future)
```csharp
public class ImageMetadata
{
    public string Id { get; set; }
    public string FileName { get; set; }
    public string OriginalUrl { get; set; }
    public string ThumbnailUrl { get; set; }
    public string MediumUrl { get; set; }
    public string Category { get; set; }
    public long FileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string AltText { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; }
    public bool IsUsed { get; set; }
    public List<string> UsedInArticles { get; set; }
}
```

## 🚀 Implementation Phases

### Phase 1: Current (Seed Data)
- ✅ MinIO container running
- ✅ Bucket auto-creation
- ✅ Mix of Unsplash URLs (external) and MinIO URLs (self-hosted)
- ✅ Basic image path support in NewsArticle entity

### Phase 2: Admin Upload API (Next)
- [ ] Create `AdminController` with upload endpoint
- [ ] Implement image validation
- [ ] Add thumbnail generation (using ImageSharp)
- [ ] Create `ImageMetadata` collection in MongoDB
- [ ] Update `MinioImageStorageService` to support variants

### Phase 3: Frontend Admin Panel
- [ ] Build admin dashboard with authentication
- [ ] Integrate WYSIWYG editor (TinyMCE/Quill)
- [ ] Create image upload UI component
- [ ] Implement drag-drop file upload
- [ ] Add image gallery management
- [ ] Image search and selection from library

### Phase 4: Advanced Features
- [ ] Image optimization (WebP conversion)
- [ ] Lazy loading placeholders (LQIP - Low Quality Image Placeholder)
- [ ] CDN integration
- [ ] Image analytics (view counts)
- [ ] Auto-tagging with AI (Azure Computer Vision)

## 💡 Best Practices

1. **Always use MinIO for new uploads** - Avoid external dependencies like Unsplash
2. **Generate alt text** - Accessibility and SEO
3. **Optimize images** - Compress before upload
4. **Use responsive images** - Serve appropriate sizes based on device
5. **Implement caching** - Cache-Control headers for static images
6. **Monitor storage** - Set alerts for bucket size limits
7. **Backup images** - Separate backup strategy for MinIO volumes

## 🔗 Related Documentation
- MinioImageStorageService: `backend/Infrastructure/Services/MinioImageStorageService.cs`
- NewsArticle Entity: `backend/Domain/Entities/NewsArticle.cs`
- Docker MinIO Setup: `docker-compose.yml`
- API Documentation: `NEWS_API_DOCUMENTATION.md`

## 📞 Questions?
Contact: DevOps Team or Backend Lead
