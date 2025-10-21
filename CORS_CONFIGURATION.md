# Backend CORS Configuration Guide

For the frontend to communicate with the backend, you need to configure CORS (Cross-Origin Resource Sharing) in the News API backend.

## 🔧 Configuration Steps

### 1. Update appsettings.json

Open `newsApi/appsettings.json` and add the `AllowedOrigins` section:

```json
{
  "DatabaseSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "NewsDb",
    "CollectionName": "News"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "NewsApi",
    "Audience": "NewsApiUsers",
    "ExpirationMinutes": 60
  },
  "AllowedOrigins": [
    "http://localhost:3000",
    "http://localhost:3001"
  ],
  "CacheSettings": {
    "DefaultExpirationMinutes": 30
  }
}
```

### 2. Update appsettings.Development.json

For development environment, open `newsApi/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedOrigins": [
    "http://localhost:3000",
    "http://localhost:3001",
    "http://127.0.0.1:3000"
  ]
}
```

### 3. Add CORS Configuration in Program.cs

If not already present, ensure CORS is configured in `newsApi/Program.cs`:

```csharp
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("AllowedOrigins")
            .Get<string[]>() ?? new[] { "http://localhost:3000" };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ... other configurations ...

var app = builder.Build();

// Use CORS before other middleware
app.UseCors("AllowFrontend");

// ... rest of middleware ...
```

### 4. For Production Deployment

Update with your production URL:

```json
{
  "AllowedOrigins": [
    "https://yourdomain.com",
    "https://www.yourdomain.com"
  ]
}
```

## ✅ Verification

### Test CORS Configuration

1. **Start the backend**:
   ```bash
   cd newsApi
   dotnet run
   ```

2. **Start the frontend**:
   ```bash
   cd web
   npm run dev
   ```

3. **Open browser console** at http://localhost:3000

4. **Check for CORS errors**:
   - If you see errors like "CORS policy: No 'Access-Control-Allow-Origin' header", CORS is not configured correctly
   - If requests work and you see data, CORS is configured correctly

### Test with Curl

```bash
# Test with curl
curl -H "Origin: http://localhost:3000" \
     -H "Access-Control-Request-Method: GET" \
     -H "Access-Control-Request-Headers: Content-Type" \
     -X OPTIONS \
     http://localhost:5000/api/news \
     -v

# Look for these headers in response:
# Access-Control-Allow-Origin: http://localhost:3000
# Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS
```

## 🔍 Troubleshooting

### Issue: CORS error in browser console

**Error**: 
```
Access to XMLHttpRequest at 'http://localhost:5000/api/news' from origin 
'http://localhost:3000' has been blocked by CORS policy
```

**Solution**:
1. Check `AllowedOrigins` includes `http://localhost:3000`
2. Restart the backend after configuration changes
3. Clear browser cache
4. Check that `app.UseCors()` is called before `app.UseAuthorization()`

### Issue: Credentials not working

**Error**: 
```
Credentials flag is 'true', but the 'Access-Control-Allow-Credentials' header is ''
```

**Solution**:
Ensure `.AllowCredentials()` is in the CORS policy:

```csharp
policy.WithOrigins(allowedOrigins)
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials();  // This line is important
```

### Issue: Preflight request failing

**Error**: 
```
Response to preflight request doesn't pass access control check
```

**Solution**:
Ensure OPTIONS method is allowed and CORS middleware is configured correctly.

## 📚 Understanding CORS

### What is CORS?

CORS (Cross-Origin Resource Sharing) is a security feature that allows or restricts web pages from making requests to a different domain than the one serving the web page.

### Why is it needed?

Without CORS:
- Browser blocks requests from `http://localhost:3000` (frontend) to `http://localhost:5000` (backend)
- This is a security measure to prevent malicious websites from accessing your API

With CORS configured:
- Backend explicitly allows requests from trusted origins
- Frontend can communicate with backend
- Other origins are still blocked

### Best Practices

1. **Development**: Allow localhost origins
2. **Production**: Only allow specific production domains
3. **Never use `AllowAnyOrigin()` with `AllowCredentials()`**
4. **Be specific**: Only allow origins you trust
5. **Use HTTPS**: In production, always use HTTPS

## 🔐 Security Considerations

### Do's

✅ Specify exact origins (not wildcards)
✅ Use environment-specific configurations
✅ Test CORS configuration thoroughly
✅ Keep `AllowedOrigins` in configuration, not hardcoded
✅ Use HTTPS in production

### Don'ts

❌ Don't use `AllowAnyOrigin()` in production
❌ Don't allow credentials with wildcard origins
❌ Don't expose internal endpoints
❌ Don't forget to restart server after config changes

## 📝 Configuration Template

Here's a complete configuration template:

**appsettings.json**:
```json
{
  "AllowedOrigins": ["http://localhost:3000"],
  "DatabaseSettings": { /* ... */ },
  "JwtSettings": { /* ... */ }
}
```

**appsettings.Development.json**:
```json
{
  "AllowedOrigins": [
    "http://localhost:3000",
    "http://localhost:3001",
    "http://127.0.0.1:3000"
  ]
}
```

**appsettings.Production.json**:
```json
{
  "AllowedOrigins": [
    "https://yourdomain.com",
    "https://www.yourdomain.com"
  ]
}
```

**Program.cs**:
```csharp
// Register CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ... other services ...

var app = builder.Build();

// Apply CORS policy BEFORE other middleware
app.UseCors("AllowFrontend");

// ... other middleware ...
```

## ✅ Checklist

Before testing the integration:

- [ ] Updated `appsettings.json` with `AllowedOrigins`
- [ ] Updated `appsettings.Development.json` with local origins
- [ ] CORS service registered in `Program.cs`
- [ ] `app.UseCors()` called before other middleware
- [ ] Backend restarted after configuration changes
- [ ] Frontend `.env.local` has correct `NEXT_PUBLIC_API_URL`
- [ ] Both servers running (backend on 5000, frontend on 3000)
- [ ] Tested in browser - no CORS errors in console

## 🚀 Quick Test

After configuration:

```bash
# Terminal 1: Start backend
cd newsApi
dotnet run

# Terminal 2: Start frontend
cd web
npm run dev

# Terminal 3: Test API call
curl -H "Origin: http://localhost:3000" \
     http://localhost:5000/api/news
```

If you see news data (not CORS error), configuration is successful! ✅

---

**Need Help?** 
- Check browser console for detailed CORS errors
- Verify configuration with the checklist above
- Ensure both services are running on correct ports
